import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { BehaviorSubject, Subscription, combineLatest } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { GamesApiService } from '../../../api-services/games/games-api.service';
import { GameListItem } from '../../../api-services/games/games-api.models';

@Component({
  selector: 'app-games-catalog',
  standalone: false,
  templateUrl: './games-catalog.component.html',
  styleUrl: './games-catalog.component.scss',
})
export class GamesCatalogComponent implements OnInit, OnDestroy {
  private gamesApi = inject(GamesApiService);

  games: GameListItem[] = [];
  genres: string[] = [];          // lista žanrova za chipove (iz prvog, nefiltriranog učitavanja)
  loading = true;
  searchTerm = '';
  selectedGenre: string | null = null;

  readonly placeholderImage = 'https://placehold.co/600x340/171a36/8d90b8?text=Loadout';
  readonly skeletons = Array.from({ length: 8 }); // 8 "praznih" kartica dok se čeka API

  // BehaviorSubject = Subject koji pamti zadnju vrijednost (i odmah je javi novim slušaocima)
  private search$ = new BehaviorSubject<string>('');
  private genre$ = new BehaviorSubject<string | null>(null);
  private sub?: Subscription;

  ngOnInit(): void {
    // combineLatest: kad god se promijeni ILI tekst pretrage ILI žanr -> novi API poziv s oba filtera
    this.sub = combineLatest([
      this.search$.pipe(debounceTime(300), distinctUntilChanged()),
      this.genre$.pipe(distinctUntilChanged()),
    ])
      .pipe(
        switchMap(([term, genre]) => {
          this.loading = true;
          return this.gamesApi.list(term, genre);
        })
      )
      .subscribe({
        next: (games) => {
          this.games = games;
          // žanrove uzmemo jednom, dok još nema filtera, da se chipovi ne mijenjaju pri filtriranju
          if (this.genres.length === 0 && !this.searchTerm && !this.selectedGenre) {
            this.genres = [...new Set(games.map((g) => g.genre).filter((g): g is string => !!g))].sort();
          }
          this.loading = false;
        },
        error: () => {
          this.games = [];
          this.loading = false;
        },
      });
  }

  onSearch(term: string): void {
    this.search$.next(term);
  }

  selectGenre(genre: string | null): void {
    this.selectedGenre = genre;
    this.genre$.next(genre);
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.search$.next('');
    this.selectGenre(null);
  }

  // Ako URL slike ne radi, browser okine (error) -> zamijenimo placeholderom
  onImageError(event: Event): void {
    const img = event.target as HTMLImageElement;
    if (img.src !== this.placeholderImage) img.src = this.placeholderImage;
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }
}
