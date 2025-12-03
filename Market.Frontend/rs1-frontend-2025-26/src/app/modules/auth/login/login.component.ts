import { Component, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { BaseComponent } from '../../../core/components/baseComponent';
import { AuthService } from '../../../core/services/auth/auth.service';
import { LoginRequest } from '../../../core/services/auth/auth.model';
import { AuthStateService } from '../../../core/services/auth/auth-state.service';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent extends BaseComponent {

  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private authState = inject(AuthStateService);
  private router = inject(Router);

  form = this.fb.group({
    email: ['admin@market.local', [Validators.required, Validators.email]],
    password: ['Admin123!', [Validators.required]],
    rememberMe: [false],
  });

  private buildLoginPayload(): LoginRequest {
    const value = this.form.value;
    return {
      email: value.email ?? '',
      password: value.password ?? '',
      fingerprint: undefined,
    };
  }

  onSubmit(): void {
    if (this.form.invalid || this.isLoading) {
      return;
    }

    this.startLoading();
    const payload = this.buildLoginPayload();

    this.authService.login(payload).subscribe({
      next: (res) => {
        // ⬇ premješteno iz komponenti u AuthStateService
        this.authState.setLogin(res);

        this.stopLoading();
        this.router.navigate(['/admin']);
      },
      error: () => {
        this.stopLoading('Neispravni kredencijali. Pokušajte ponovo.');
      },
    });
  }
}
