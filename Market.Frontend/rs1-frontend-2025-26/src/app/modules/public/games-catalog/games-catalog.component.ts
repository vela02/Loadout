import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { BehaviorSubject, Subscription, combineLatest } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { CatalogFilters, GamesApiService } from '../../../api-services/games/games-api.service';
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

  // ---------- NAPREDNI FILTERI ----------
  showFilters = false;
  readonly ratingOptions = [1, 2, 3, 4, 5];
  readonly contentTypes = [0, 1, 2, 3];   // ključevi GAMES.CONTENT_TYPES.n
  filters: CatalogFilters = { minPrice: null, maxPrice: null, minRating: null, type: null, releasedFrom: null, releasedTo: null };
  priceError = '';
  dateError = '';
  private filters$ = new BehaviorSubject<CatalogFilters>({ ...this.filters });
  private sub?: Subscription;

  ngOnInit(): void {
    // combineLatest: kad god se promijeni ILI tekst pretrage ILI žanr -> novi API poziv s oba filtera
    this.sub = combineLatest([
      this.search$.pipe(debounceTime(300), distinctUntilChanged()),
      this.genre$.pipe(distinctUntilChanged()),
      this.filters$.pipe(debounceTime(300)),          // cijena se kuca -> sačekaj da korisnik završi
    ])
      .pipe(
        switchMap(([term, genre, filters]) => {
          this.loading = true;
          return this.gamesApi.list(term, genre, filters);
        })
      )
      .subscribe({
        next: (games) => {
          this.games = games;
          // žanrove uzmemo jednom, dok još nema filtera, da se chipovi ne mijenjaju pri filtriranju
          if (this.genres.length === 0 && !this.searchTerm && !this.selectedGenre && this.activeFilterCount === 0) {
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
    this.resetAdvancedFilters();
  }

  // ---------- NAPREDNI FILTERI ----------
  /** Poziva se na svaku promjenu u panelu filtera */
  applyFilters(): void {
    const { minPrice, maxPrice } = this.filters;
    // FE validacija: cijene ne smiju biti negativne, a "od" ne smije biti veće od "do"
    if ((minPrice != null && minPrice < 0) || (maxPrice != null && maxPrice < 0)) {
      this.priceError = 'GAMES.FILTERS.ERR_NEGATIVE';
      return;
    }
    if (minPrice != null && maxPrice != null && minPrice > maxPrice) {
      this.priceError = 'GAMES.FILTERS.ERR_RANGE';
      return;
    }
    this.priceError = '';

    // Datum izlaska: "od" ne smije biti poslije "do"
    const { releasedFrom, releasedTo } = this.filters;
    if (releasedFrom && releasedTo && releasedFrom > releasedTo) {   // "YYYY-MM-DD" se ispravno poredi kao tekst
      this.dateError = 'GAMES.FILTERS.ERR_DATE_RANGE';
      return;
    }
    this.dateError = '';
    this.filters$.next({ ...this.filters });
  }

  setRating(r: number | null): void {
    this.filters.minRating = this.filters.minRating === r ? null : r; // drugi klik na istu ocjenu je isključi
    this.applyFilters();
  }

  setType(t: number | null): void {
    this.filters.type = t;
    this.applyFilters();
  }

  /** Brzi izbori za datum izlaska */
  setReleasePreset(preset: 'upcoming' | 'lastYear' | null): void {
    const today = new Date();
    const iso = (d: Date) => d.toISOString().slice(0, 10);
    if (preset === 'upcoming') {
      // sutra pa nadalje = igre koje još nisu izašle (pre-order)
      const tomorrow = new Date(today); tomorrow.setDate(today.getDate() + 1);
      this.filters.releasedFrom = iso(tomorrow);
      this.filters.releasedTo = null;
    } else if (preset === 'lastYear') {
      const yearAgo = new Date(today); yearAgo.setFullYear(today.getFullYear() - 1);
      this.filters.releasedFrom = iso(yearAgo);
      this.filters.releasedTo = iso(today);
    } else {
      this.filters.releasedFrom = null;
      this.filters.releasedTo = null;
    }
    this.applyFilters();
  }

  resetAdvancedFilters(): void {
    this.filters = { minPrice: null, maxPrice: null, minRating: null, type: null, releasedFrom: null, releasedTo: null };
    this.priceError = '';
    this.dateError = '';
    this.filters$.next({ ...this.filters });
  }

  /** Broj aktivnih naprednih filtera (za bedž na dugmetu "Filteri") */
  get activeFilterCount(): number {
    const f = this.filters;
    return [f.minPrice, f.maxPrice, f.minRating, f.type, f.releasedFrom, f.releasedTo]
      .filter((v) => v !== null && v !== undefined && v !== '').length;
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
