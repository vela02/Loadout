// src/app/modules/auth/logout/logout.component.ts
import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import {
  trigger,
  transition,
  style,
  animate,
} from '@angular/animations';
import { AuthService } from '../../../core/services/auth/auth.service';
import { AuthStateService } from '../../../core/services/auth/auth-state.service';
import { LogoutRequest } from '../../../core/services/auth/auth.model';

@Component({
  selector: 'app-logout',
  standalone: false,
  templateUrl: './logout.component.html',
  styleUrl: './logout.component.scss',
  animations: [
    trigger('fadeInUp', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(10px)' }),
        animate(
          '400ms ease-out',
          style({ opacity: 1, transform: 'translateY(0)' })
        ),
      ]),
    ]),
  ],
})
export class LogoutComponent implements OnInit {

  private router = inject(Router);
  private authService = inject(AuthService);
  private authState = inject(AuthStateService);

  countdownSeconds = 2;

  ngOnInit(): void {
    const refreshToken = this.authState.refreshToken;

    // Ako imamo refreshToken → pokušaj server-side logout
    if (refreshToken) {
      const payload: LogoutRequest = { refreshToken };

      this.authService.logout(payload).subscribe({
        next: () => this.finishLogout(),
        error: () => this.finishLogout(), // čak i ako padne API, očistimo lokalno
      });
    } else {
      // Nema tokena, samo lokalno očistimo i redirect
      this.finishLogout();
    }
  }

  private finishLogout(): void {
    // 1) Očisti lokalno stanje (svi tokeni)
    this.authState.clear();

    // 2) Countdown + redirect na /login
    const intervalId = setInterval(() => {
      this.countdownSeconds--;

      if (this.countdownSeconds <= 0) {
        clearInterval(intervalId);
        this.router.navigate(['/login']);
      }
    }, 1000);
  }
}
