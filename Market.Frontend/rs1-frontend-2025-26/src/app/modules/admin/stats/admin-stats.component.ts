import { Component, OnInit, inject } from '@angular/core';
import { GamesApiService } from '../../../api-services/games/games-api.service';
import { DashboardStats } from '../../../api-services/games/games-api.models';

@Component({
  selector: 'app-admin-stats',
  standalone: false,
  templateUrl: './admin-stats.component.html',
  styleUrl: './admin-stats.component.scss',
})
export class AdminStatsComponent implements OnInit {
  private gamesApi = inject(GamesApiService);

  stats: DashboardStats | null = null;
  error = '';

  ngOnInit(): void {
    // Ruta /admin/* je zaštićena guardom (samo Administrator),
    // a i backend endpoint ima [Authorize(Roles = "Administrator")].
    this.gamesApi.getStats().subscribe({
      next: (s) => (this.stats = s),
      error: () => (this.error = 'GAMES.STATS.LOAD_ERROR'),
    });
  }
}
