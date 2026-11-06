import { Component, OnInit, inject } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { CartStateService } from '../../../api-services/games/cart-state.service';
import { CurrentUserService } from '../../../core/services/auth/current-user.service';

/**
 * Zajedničko zaglavlje za katalog i stranicu igre:
 * lijevo logo (link na početnu), desno izbor jezika (isti princip kao na Velidovom landing pageu).
 */
@Component({
  selector: 'app-store-header',
  standalone: false,
  templateUrl: './store-header.component.html',
  styleUrl: './store-header.component.scss',
})
export class StoreHeaderComponent implements OnInit {
  private translate = inject(TranslateService);
  private cartState = inject(CartStateService);
  private currentUser = inject(CurrentUserService);

  cartCount$ = this.cartState.count$;           // broj stavki za bedž na ikoni korpe
  isLoggedIn = this.currentUser.isAuthenticated; // signal (čita se kao isLoggedIn())

  // Jezik se pamti u localStorage pod istim ključem kao kod Velida -> isti izbor na svim stranicama
  currentLang: string = localStorage.getItem('language') || this.translate.currentLang || 'bs';

  languages = [
    { code: 'bs', name: 'Bosanski', flag: '🇧🇦' },
    { code: 'en', name: 'English', flag: '🇬🇧' },
  ];

  constructor() {
    // Ako je korisnik ranije izabrao jezik, primijeni ga i ovdje
    if (this.translate.currentLang !== this.currentLang) {
      this.translate.use(this.currentLang);
    }
  }

  ngOnInit(): void {
    this.cartState.refresh(); // učitaj broj stavki u korpi
  }

  switchLanguage(code: string): void {
    this.currentLang = code;
    this.translate.use(code);
    localStorage.setItem('language', code);
  }

  get current() {
    return this.languages.find((l) => l.code === this.currentLang);
  }
}
