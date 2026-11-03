import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { GamesApiService } from '../../../api-services/games/games-api.service';
import { GameDetails } from '../../../api-services/games/games-api.models';
import { CurrentUserService } from '../../../core/services/auth/current-user.service';
import { ToasterService } from '../../../core/services/toaster.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-game-details',
  standalone: false,
  templateUrl: './game-details.component.html',
  styleUrl: './game-details.component.scss',
})
export class GameDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private gamesApi = inject(GamesApiService);
  private currentUser = inject(CurrentUserService);
  private toaster = inject(ToasterService);
  private translate = inject(TranslateService); // za prevod poruka u TS kodu (toaster)

  game: GameDetails | null = null;
  loading = true;
  error = '';
  buying = false;

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
        this.toaster.success(this.translate.instant(this.game!.isPreOrder
          ? 'GAMES.DETAILS.PREORDERED'
          : 'GAMES.DETAILS.ADDED_TO_CART'));
      },
      error: () => {
        this.buying = false;
        this.toaster.error(this.translate.instant('GAMES.DETAILS.ADD_FAILED'));
      },
    });
  }

  onImageError(event: Event): void {
    const img = event.target as HTMLImageElement;
    if (img.src !== this.placeholderImage) img.src = this.placeholderImage;
  }
}
