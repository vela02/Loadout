import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { switchMap } from 'rxjs/operators';
import { GamesApiService } from '../../../api-services/games/games-api.service';
import { GameDetails } from '../../../api-services/games/games-api.models';
import { CurrentUserService } from '../../../core/services/auth/current-user.service';
import { ToasterService } from '../../../core/services/toaster.service';
import { TranslateService } from '@ngx-translate/core';
import { CartStateService } from '../../../api-services/games/cart-state.service';

@Component({
  selector: 'app-game-details',
  standalone: false,
  templateUrl: './game-details.component.html',
  styleUrl: './game-details.component.scss',
})
export class GameDetailsComponent implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private sanitizer = inject(DomSanitizer);
  private gamesApi = inject(GamesApiService);
  private currentUser = inject(CurrentUserService);
  private toaster = inject(ToasterService);
  private translate = inject(TranslateService); // za prevod poruka u TS kodu (toaster)
  private cartState = inject(CartStateService);

  game: GameDetails | null = null;
  loading = true;
  error = '';
  buying = false;
  saving = false;
  showShare = false;   // panel s QR kodom za dijeljenje

  // Trailer: modal s YouTube playerom
  trailerEmbed: SafeResourceUrl | null = null;
  showTrailer = false;

  // Popup notifikacija nakon dodavanja (umjesto običnog toastera)
  popup: 'cart' | 'saved' | null = null;
  private popupTimer?: ReturnType<typeof setTimeout>;

  readonly placeholderImage = 'https://placehold.co/800x450/171a36/8d90b8?text=Loadout';
  readonly stars = [1, 2, 3, 4, 5];

  ngOnInit(): void {
    // paramMap "sluša" URL: svaki put kad se :id promijeni, pozovemo API za tu igru.
    // switchMap otkaže prethodni zahtjev ako korisnik brzo pređe na drugu igru.
    this.route.paramMap
      .pipe(
        switchMap((params) => {
          this.loading = true;
          this.error = '';
          return this.gamesApi.getById(Number(params.get('id')));
        })
      )
      .subscribe({
        next: (game) => {
          this.game = game;
          this.loading = false;
        },
        error: () => {
          this.error = 'GAMES.DETAILS.NOT_FOUND';
          this.loading = false;
        },
      });
  }

  /** Ključ prevoda za tip sadržaja: GAMES.CONTENT_TYPES.0 = Igra/Game, 1 = DLC... */
  get contentTypeKey(): string {
    return `GAMES.CONTENT_TYPES.${this.game?.contentType ?? 0}`;
  }

  /** Kupi ili rezerviši (pre-order). Neprijavljenog korisnika šaljemo na login. */
  buy(): void {
    const user = this.currentUser.snapshot;
    if (!user) {
      this.toaster.info(this.translate.instant('GAMES.DETAILS.LOGIN_TO_BUY'));
      this.router.navigate(['/auth/login']);
      return;
    }
    if (!this.game) return;

    this.buying = true;
    this.gamesApi.addToCart(user.userId, this.game.id).subscribe({
      next: () => {
        this.buying = false;
        this.cartState.refresh(); // osvježi broj na ikoni korpe
        this.showPopup('cart');
      },
      error: () => {
        this.buying = false;
        this.toaster.error(this.translate.instant('GAMES.DETAILS.ADD_FAILED'));
      },
    });
  }

  // ---------- TRAILER ----------

  /** Izvuče YouTube video ID (11 znakova) iz watch / youtu.be / embed / shorts linka */
  private youtubeId(url: string | null): string | null {
    if (!url) return null;
    const m = url.match(/(?:youtube\.com\/(?:watch\?v=|embed\/|shorts\/)|youtu\.be\/)([A-Za-z0-9_-]{11})/);
    return m ? m[1] : null;
  }

  /** Link za otvaranje na YouTubeu: tačan video ili pretraga "naziv + trailer" */
  get trailerWatchUrl(): string {
    const id = this.youtubeId(this.game?.trailerUrl ?? null);
    return id
      ? `https://www.youtube.com/watch?v=${id}`
      : `https://www.youtube.com/results?search_query=${encodeURIComponent((this.game?.title ?? '') + ' official trailer')}`;
  }

  openTrailer(): void {
    const id = this.youtubeId(this.game?.trailerUrl ?? null);
    if (!id) {
      // nema trailera u bazi -> YouTube pretraga u novom tabu
      window.open(this.trailerWatchUrl, '_blank', 'noopener');
      return;
    }
    // SIGURNOST: URL slažemo sami samo od provjerenog ID-a, tek onda ga označimo kao siguran
    this.trailerEmbed = this.sanitizer.bypassSecurityTrustResourceUrl(
      `https://www.youtube-nocookie.com/embed/${id}?autoplay=1&rel=0`);
    this.showTrailer = true;
  }

  closeTrailer(): void {
    this.showTrailer = false;
    this.trailerEmbed = null; // uklanja iframe -> video se zaustavi
  }

  /** Puni link na ovu igru – ide u QR kod i u "Kopiraj link" */
  get shareUrl(): string {
    return `${window.location.origin}/games/${this.game?.id}`;
  }

  /** "Sačuvaj za kasnije" direktno sa stranice igre */
  saveForLater(): void {
    const user = this.currentUser.snapshot;
    if (!user) {
      this.toaster.info(this.translate.instant('GAMES.DETAILS.LOGIN_TO_BUY'));
      this.router.navigate(['/auth/login']);
      return;
    }
    if (!this.game) return;

    this.saving = true;
    this.gamesApi.saveForLater(user.userId, this.game.id).subscribe({
      next: () => {
        this.saving = false;
        this.cartState.refresh();
        this.showPopup('saved');
      },
      error: () => {
        this.saving = false;
        this.toaster.error(this.translate.instant('GAMES.DETAILS.ADD_FAILED'));
      },
    });
  }

  /** Prikaži popup i automatski ga sakrij nakon 5 sekundi */
  private showPopup(type: 'cart' | 'saved'): void {
    clearTimeout(this.popupTimer);
    this.popup = null;
    // setTimeout(0) -> Angular ponovo iscrta popup, pa animacija krene ispočetka
    setTimeout(() => {
      this.popup = type;
      this.popupTimer = setTimeout(() => (this.popup = null), 5000);
    });
  }

  closePopup(): void {
    clearTimeout(this.popupTimer);
    this.popup = null;
  }

  goToCart(): void {
    this.closePopup();
    this.router.navigate(['/cart']);
  }

  ngOnDestroy(): void {
    clearTimeout(this.popupTimer);
  }

  onImageError(event: Event): void {
    const img = event.target as HTMLImageElement;
    if (img.src !== this.placeholderImage) img.src = this.placeholderImage;
  }
}
