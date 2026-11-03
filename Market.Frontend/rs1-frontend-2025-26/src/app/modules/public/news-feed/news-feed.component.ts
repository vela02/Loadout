import { Component, OnInit, inject } from '@angular/core';
import { GamesApiService } from '../../../api-services/games/games-api.service';
import { Announcement } from '../../../api-services/games/games-api.models';

@Component({
  selector: 'app-news-feed',
  standalone: false,
  templateUrl: './news-feed.component.html',
})
export class NewsFeedComponent implements OnInit {
  private gamesApi = inject(GamesApiService);
  news: Announcement[] = [];

  ngOnInit(): void {
    // Backend vraća samo aktivne obavijesti (neaktivne admin isključi preko "toggle")
    this.gamesApi.getAnnouncements().subscribe({
      next: (items) => (this.news = items),
      error: () => (this.news = []),
    });
  }

  // Boja bedža zavisno od tipa obavijesti
  badgeClass(type: string): string {
    switch ((type || '').toLowerCase()) {
      case 'sale': return 'bg-success';
      case 'maintenance': return 'bg-danger';
      default: return 'bg-info text-dark';
    }
  }
}
