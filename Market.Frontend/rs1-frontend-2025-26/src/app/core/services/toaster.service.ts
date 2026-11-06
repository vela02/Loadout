import { Injectable, inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NeonToastComponent, ToastData, ToastType } from '../components/neon-toast/neon-toast.component';

/**
 * Global toaster service for displaying notifications.
 * Javne metode su ISTE kao prije (success/error/warning/info),
 * ali se umjesto standardnog snackbara prikazuje jedinstvena neon notifikacija
 * (NeonToastComponent) – isti izgled u cijeloj aplikaciji.
 */
@Injectable({
  providedIn: 'root'
})
export class ToasterService {
  private snackBar = inject(MatSnackBar);
  private readonly defaultDuration = 4000;

  success(message: string, duration?: number): void {
    this.show('success', message, duration);
  }

  error(message: string, duration?: number): void {
    this.show('error', message, duration ?? 5000); // greške ostaju malo duže
  }

  warning(message: string, duration?: number): void {
    this.show('warning', message, duration);
  }

  info(message: string, duration?: number): void {
    this.show('info', message, duration);
  }

  private show(type: ToastType, message: string, duration?: number): void {
    const ms = duration ?? this.defaultDuration;
    this.snackBar.openFromComponent<NeonToastComponent, ToastData>(NeonToastComponent, {
      data: { type, message, duration: ms },
      duration: ms,
      horizontalPosition: 'end',
      verticalPosition: 'top',
      panelClass: ['neon-toast-panel'],   // globalni stil uklanja standardnu Material "kutiju"
    });
  }
}
