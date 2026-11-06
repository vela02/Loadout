import { Component, ElementRef, NgZone, OnDestroy, OnInit, ViewChild, inject } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { loadStripe, Stripe, StripeCardElement } from '@stripe/stripe-js';
import { AbstractControl, FormBuilder, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { GamesApiService } from '../../../api-services/games/games-api.service';
import { CartStateService } from '../../../api-services/games/cart-state.service';
import { CartItem, CheckoutResult, OrderHistory } from '../../../api-services/games/games-api.models';
import { CurrentUserService } from '../../../core/services/auth/current-user.service';
import { ToasterService } from '../../../core/services/toaster.service';
import { CountryOption, getCountries } from '../../../core/models/countries';
import { ReceiptPdfService } from '../../../core/services/receipt-pdf.service';
import { PaymentsApiService } from '../../../api-services/payments/payments-api.service';

/**
 * Checkout WIZARD u 4 koraka:
 *   1. Korpa      – pregled, ukloni, "sačuvaj za kasnije"
 *   2. Podaci     – podaci kupca (reactive forma + validacija)
 *   3. Plaćanje   – STRIPE kartica (Stripe Elements) + uslovi
 *   4. Potvrda    – rezultat narudžbe i licencni ključevi
 * Korisnik ne može na sljedeći korak dok trenutni nije ispravan.
 */
@Component({
  selector: 'app-checkout',
  standalone: false,
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.scss',
})
export class CheckoutComponent implements OnInit, OnDestroy {
  private fb = inject(FormBuilder);
  private gamesApi = inject(GamesApiService);
  private cartState = inject(CartStateService);
  private currentUser = inject(CurrentUserService);
  private toaster = inject(ToasterService);
  private translate = inject(TranslateService);
  private receiptPdf = inject(ReceiptPdfService);
  private paymentsApi = inject(PaymentsApiService);
  private zone = inject(NgZone); // za izmjene koje dolaze iz Stripe.js (izvan Angulara)

  // ---------- STRIPE ----------
  private stripe: Stripe | null = null;
  private card: StripeCardElement | null = null;
  stripeReady = false;
  stripeMissing = false;      // backend nema podešen publishable key
  testMode = false;           // pk_test_... -> prikazujemo testne kartice (kod pk_live_ se same sakriju)
  showTestCards = false;
  cardComplete = false;       // Stripe javlja kad su broj, datum i CVC ispravno uneseni
  cardError = '';             // poruka greške s kartice (npr. "Broj kartice nije ispravan")
  paymentError = '';          // greška pri samom plaćanju (odbijena kartica i sl.)

  /**
   * Div u koji Stripe "montira" svoje polje za karticu.
   * Setter se pozove svaki put kad korak 3 postane vidljiv (i kad nestane -> undefined).
   */
  @ViewChild('cardHost') set cardHost(el: ElementRef<HTMLDivElement> | undefined) {
    if (el) this.mountCard(el.nativeElement);
  }

  readonly steps = ['GAMES.CHECKOUT.STEP_CART', 'GAMES.CHECKOUT.STEP_DETAILS', 'GAMES.CHECKOUT.STEP_PAYMENT', 'GAMES.CHECKOUT.STEP_DONE'];
  readonly placeholderImage = 'https://placehold.co/160x90/171a36/8d90b8?text=Loadout';

  step = 0;
  loading = true;
  processing = false;

  items: CartItem[] = [];
  result: CheckoutResult | null = null;
  orders: OrderHistory[] = [];   // kreirane narudžbe s ključevima (za korak 4)

  // Sve države svijeta, nazivi na trenutnom jeziku aplikacije
  countries: CountryOption[] = getCountries(this.translate.currentLang || 'bs');

  private get userId(): number {
    return this.currentUser.snapshot!.userId; // ruta je zaštićena guardom -> korisnik postoji
  }

  // ---------- FORME (korak 2 i 3) ----------
  billingForm = this.fb.nonNullable.group({
    fullName: ['', [Validators.required, Validators.minLength(3)]],
    // Validators.email prihvata i "ime@gmail" (bez .com), pa dodajemo regex:
    // nešto@domena.tld, bez razmaka, i TLD od najmanje 2 slova (.ba, .com, .info)
    email: [this.currentUser.snapshot?.email ?? '', [
      Validators.required,
      Validators.email,
      Validators.pattern(/^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$/),
    ]],
    phone: ['', Validators.pattern(/^\+?[0-9\s\/-]{6,20}$/)],          // opcionalno, npr. +387 61 123 456
    address: ['', [Validators.required, Validators.minLength(3)]],
    city: ['', Validators.required],
    postalCode: ['', [Validators.required, Validators.pattern(/^[A-Za-z0-9\s-]{3,10}$/)]], // 71000, 10115, SW1A 1AA
    country: ['', Validators.required],
  });

  // Podatke kartice NE držimo u našoj formi – njih unosi Stripe-ovo polje (PCI sigurnost).
  paymentForm = this.fb.nonNullable.group({
    acceptTerms: [false, Validators.requiredTrue],
  });

  // ---------- IZRAČUNATE VRIJEDNOSTI ----------
  get activeItems(): CartItem[] {
    return this.items.filter((i) => !i.isSavedForLater);
  }

  get savedItems(): CartItem[] {
    return this.items.filter((i) => i.isSavedForLater);
  }

  get total(): number {
    return this.activeItems.reduce((sum, i) => sum + i.price * i.quantity, 0);
  }

  get hasPreOrder(): boolean {
    return this.activeItems.some((i) => i.isPreOrder);
  }

  ngOnInit(): void {
    this.loadCart();

    // Kad korisnik promijeni jezik, prevedi i nazive država
    this.translate.onLangChange.subscribe((e) => (this.countries = getCountries(e.lang)));

    this.initStripe();
  }

  // ---------- STRIPE: inicijalizacija ----------
  private async initStripe(): Promise<void> {
    try {
      const { publishableKey } = await firstValueFrom(this.paymentsApi.getConfig());
      if (!publishableKey) {
        this.stripeMissing = true;
        return;
      }
      this.testMode = publishableKey.startsWith('pk_test_');
      this.stripe = await loadStripe(publishableKey);   // učitava Stripe.js sa js.stripe.com
      this.stripeReady = !!this.stripe;
    } catch {
      this.stripeMissing = true;
    }
  }

  /** Napravi (jednom) Stripe Card Element i montiraj ga u zadani div */
  private mountCard(host: HTMLElement): void {
    if (!this.stripe) {
      // Stripe.js se još učitava -> pokušaj ponovo za trenutak
      setTimeout(() => this.stripe && this.mountCard(host), 300);
      return;
    }
    if (!this.card) {
      const elements = this.stripe.elements({ locale: (this.translate.currentLang === 'en' ? 'en' : 'hr') as any });
      this.card = elements.create('card', {
        hidePostalCode: true,          // poštanski broj već imamo iz koraka 2
        style: {
          base: {
            color: '#e8e9ff',
            iconColor: '#22d3ee',
            fontFamily: 'Roboto, "Helvetica Neue", sans-serif',
            fontSize: '16px',
            '::placeholder': { color: '#8d90b8' },
          },
          invalid: { color: '#f87171', iconColor: '#f87171' },
        },
      });
      // Stripe javlja stanje unosa: da li je kartica kompletna i ima li grešku
      this.card.on('change', (e) => {
        this.zone.run(() => {
          this.cardComplete = e.complete;
          this.cardError = e.error?.message ?? '';
        });
      });
    } else {
      this.card.unmount();             // vraćanje na korak 3 -> ponovo montiraj u novi div
    }
    this.card.mount(host);
  }

  loadCart(): void {
    this.loading = true;
    this.gamesApi.getCart(this.userId).subscribe({
      next: (items) => {
        this.items = items;
        this.cartState.setCount(this.activeItems.length);
        this.loading = false;
      },
      error: () => (this.loading = false),
    });
  }

  // ---------- KORAK 1: akcije nad korpom ----------
  remove(item: CartItem): void {
    this.gamesApi.removeFromCart(this.userId, item.cartItemId).subscribe(() => this.loadCart());
  }

  toggleSaveForLater(item: CartItem): void {
    this.gamesApi
      .setSavedForLater(this.userId, item.cartItemId, !item.isSavedForLater)
      .subscribe(() => this.loadCart());
  }

  // ---------- NAVIGACIJA KROZ KORAKE ----------
  next(): void {
    if (this.step === 0 && this.activeItems.length === 0) return;

    if (this.step === 1 && this.billingForm.invalid) {
      this.billingForm.markAllAsTouched(); // prikaži greške na svim poljima
      return;
    }

    if (this.step === 2) {
      if (!this.cardComplete) {
        this.cardError = this.cardError || this.translate.instant('GAMES.CHECKOUT.ERR_CARD_INCOMPLETE');
      }
      if (this.paymentForm.invalid || !this.cardComplete) {
        this.paymentForm.markAllAsTouched();
        return;
      }
      this.placeOrder();
      return;
    }

    this.step++;
  }

  back(): void {
    if (this.step > 0 && this.step < 3) this.step--;
  }

  /** Može li se kliknuti na korak u indikatoru (samo na već završene korake) */
  canGoTo(index: number): boolean {
    return index < this.step && this.step < 3;
  }

  goTo(index: number): void {
    if (this.canGoTo(index)) this.step = index;
  }

  // ---------- KORAK 3 -> 4: STRIPE PLAĆANJE ----------
  /**
   * 1) backend otvori PaymentIntent (iznos računa server)
   * 2) Stripe.js pošalje karticu DIREKTNO Stripe-u i potvrdi plaćanje (uključujući 3D Secure ako treba)
   * 3) backend provjeri kod Stripe-a da je plaćeno, kreira narudžbe i spremi transakciju
   */
  private async placeOrder(): Promise<void> {
    if (!this.stripe || !this.card) return;
    this.processing = true;
    this.paymentError = '';

    try {
      // 1) PaymentIntent
      const intent = await firstValueFrom(this.paymentsApi.createIntent());

      // 2) Potvrda plaćanja kod Stripe-a, s podacima kupca iz koraka 2
      const b = this.billingForm.getRawValue();
      const { error, paymentIntent } = await this.stripe.confirmCardPayment(intent.clientSecret, {
        payment_method: {
          card: this.card,
          billing_details: {
            name: b.fullName,
            email: b.email,
            phone: b.phone || undefined,
            address: { line1: b.address, city: b.city, postal_code: b.postalCode, country: b.country },
          },
        },
      });

      if (error) {
        // npr. "Your card was declined." – poruka dolazi od Stripe-a.
        // zone.run -> Angular odmah osvježi ekran (inače bi ostalo "Obrada...")
        this.zone.run(() => {
          this.paymentError = error.message ?? this.translate.instant('GAMES.CHECKOUT.ERROR');
          this.processing = false;
        });
        return;
      }

      // 3) Backend potvrda + kreiranje narudžbi
      const result = await firstValueFrom(this.paymentsApi.confirm(paymentIntent!.id));
      this.result = result;
      this.cartState.refresh();

      // Učitaj historiju i prikaži upravo kreirane narudžbe (s licencnim ključevima)
      const orders = await firstValueFrom(this.gamesApi.getOrderHistory(this.userId));
      const ids = result.orderIds?.length ? result.orderIds : [result.orderId];
      this.orders = orders.filter((o) => ids.includes(o.id));
      this.toaster.success(this.translate.instant('GAMES.CHECKOUT.PAID_OK'));
      this.finish();
    } catch (err) {
      const msg = (err as HttpErrorResponse)?.error?.message;
      this.paymentError = msg || this.translate.instant('GAMES.CHECKOUT.ERROR');
      this.processing = false;
    }
  }

  private finish(): void {
    this.processing = false;
    this.step = 3;
    this.loadCart(); // u korpi ostaju samo "za kasnije" stavke
  }

  // ---------- POMOĆNO ----------
  /** Za prikaz greške: polje je nevalidno i korisnik ga je već dirao */
  invalid(form: 'billing' | 'payment', field: string): boolean {
    // Dvije forme imaju različite tipove polja, pa ih tretiramo kao opšti AbstractControl
    const group: AbstractControl = form === 'billing' ? this.billingForm : this.paymentForm;
    const control = group.get(field);
    return !!control && control.invalid && control.touched;
  }

  /** PDF potvrda za jednu narudžbu (parametar: narudžba/ID) */
  downloadPdf(order: OrderHistory): void {
    this.receiptPdf.download(order, this.billingForm.controls.email.value);
  }

  onImageError(event: Event): void {
    const img = event.target as HTMLImageElement;
    if (img.src !== this.placeholderImage) img.src = this.placeholderImage;
  }

  ngOnDestroy(): void {
    this.card?.destroy();   // oslobodi Stripe iframe kad napustimo stranicu
  }
}
