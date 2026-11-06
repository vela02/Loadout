import { Component, inject } from '@angular/core';
import { MAT_SNACK_BAR_DATA, MatSnackBarRef } from '@angular/material/snack-bar';
import { MatIconModule } from '@angular/material/icon';

export type ToastType = 'success' | 'error' | 'warning' | 'info';

export interface ToastData {
  type: ToastType;
  message: string;
  duration: number;   // ms – koristi se i za dužinu animacije trake
}

/**
 * Jedinstvena "neon" notifikacija za cijelu aplikaciju.
 * Prikazuje je ToasterService preko MatSnackBar.openFromComponent(),
 * tako da svi postojeći pozivi (toaster.success/error/...) automatski dobiju novi izgled.
 */
@Component({
  selector: 'app-neon-toast',
  standalone: true,
  imports: [MatIconModule],
  template: `
    <div class="toast" [class]="'toast toast-' + data.type" role="status">
      <span class="icon-wrap"><mat-icon>{{ icon }}</mat-icon></span>
      <span class="message">{{ data.message }}</span>
      <button type="button" class="close" (click)="ref.dismiss()" aria-label="Close">
        <mat-icon>close</mat-icon>
      </button>
      <span class="progress" [style.animation-duration.ms]="data.duration"></span>
    </div>
  `,
  styles: [`
    :host { display: block; }

    .toast {
      --accent: #22d3ee;
      position: relative;
      overflow: hidden;
      display: flex;
      align-items: center;
      gap: 12px;
      min-width: 300px;
      max-width: 420px;
      padding: 14px 14px 16px;
      border-radius: 14px;
      border: 1px solid color-mix(in srgb, var(--accent) 55%, transparent);
      background: rgba(23, 26, 54, 0.92);
      backdrop-filter: blur(14px);
      color: #e8e9ff;
      font-family: Roboto, 'Helvetica Neue', sans-serif;
      box-shadow:
        0 0 28px color-mix(in srgb, var(--accent) 30%, transparent),
        0 18px 40px rgba(0, 0, 0, 0.55);
      animation: toast-in 0.35s cubic-bezier(0.2, 0.9, 0.3, 1.2);
    }

    .toast-success { --accent: #22d3ee; }
    .toast-info    { --accent: #a78bfa; }
    .toast-warning { --accent: #fbbf24; }
    .toast-error   { --accent: #f87171; }

    .icon-wrap {
      flex: 0 0 36px;
      height: 36px;
      border-radius: 50%;
      display: grid;
      place-items: center;
      background: color-mix(in srgb, var(--accent) 16%, transparent);
      color: var(--accent);
    }
    .icon-wrap mat-icon {
      font-size: 22px; width: 22px; height: 22px;
      filter: drop-shadow(0 0 6px var(--accent));
    }

    .message {
      flex: 1;
      font-size: 0.95rem;
      font-weight: 600;
      line-height: 1.4;
    }

    .close {
      border: none;
      background: none;
      padding: 4px;
      border-radius: 8px;
      color: #8d90b8;
      cursor: pointer;
      display: grid;
      place-items: center;
    }
    .close:hover { color: #e8e9ff; background: rgba(255, 255, 255, 0.06); }
    .close mat-icon { font-size: 18px; width: 18px; height: 18px; }

    /* traka na dnu koja se smanjuje = koliko još notifikacija ostaje */
    .progress {
      position: absolute;
      left: 0;
      bottom: 0;
      height: 3px;
      width: 100%;
      background: linear-gradient(90deg, var(--accent), #e040fb);
      transform-origin: left;
      animation-name: toast-progress;
      animation-timing-function: linear;
      animation-fill-mode: forwards;
    }

    @keyframes toast-in {
      from { opacity: 0; transform: translateX(40px) scale(0.95); }
      to   { opacity: 1; transform: translateX(0) scale(1); }
    }
    @keyframes toast-progress {
      from { transform: scaleX(1); }
      to   { transform: scaleX(0); }
    }
  `],
})
export class NeonToastComponent {
  data = inject<ToastData>(MAT_SNACK_BAR_DATA);
  ref = inject(MatSnackBarRef<NeonToastComponent>);

  get icon(): string {
    switch (this.data.type) {
      case 'success': return 'check_circle';
      case 'error': return 'error';
      case 'warning': return 'warning';
      default: return 'info';
    }
  }
}
