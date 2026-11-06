import { Injectable, inject } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { GamesApiService } from './games-api.service';
import { CurrentUserService } from '../../core/services/auth/current-user.service';

/**
 * Dijeljeno stanje korpe: broj aktivnih stavki.
 * Zaglavlje (ikona korpe) se pretplati na count$, a stranice koje mijenjaju korpu pozovu refresh().
 * providedIn: 'root' = jedna instanca za cijelu aplikaciju (singleton), pa svi vide isti broj.
 */
@Injectable({ providedIn: 'root' })
export class CartStateService {
  private gamesApi = inject(GamesApiService);
  private currentUser = inject(CurrentUserService);

  private countSubject = new BehaviorSubject<number>(0);
  readonly count$ = this.countSubject.asObservable();

  refresh(): void {
    const user = this.currentUser.snapshot;
    if (!user) {
      this.countSubject.next(0);
      return;
    }
    this.gamesApi.getCart(user.userId).subscribe({
      next: (items) => this.setCount(items.filter((i) => !i.isSavedForLater).length),
      error: () => this.countSubject.next(0),
    });
  }

  setCount(count: number): void {
    this.countSubject.next(count);
  }
}
