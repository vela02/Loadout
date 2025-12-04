import { Component, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { BaseComponent } from '../../../core/components/baseComponent';
import { AuthService } from '../../../feature-services/auth/auth.service';
import { LoginCommand } from '../../../api-services/auth/auth-api.model';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent extends BaseComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  form = this.fb.group({
    email: ['admin@market.local', [Validators.required, Validators.email]],
    password: ['Admin123!', [Validators.required]],
    rememberMe: [false]
  });

  onSubmit(): void {
    if (this.form.invalid || this.isLoading) return;

    this.startLoading();

    const payload: LoginCommand = {
      email: this.form.value.email ?? '',
      password: this.form.value.password ?? '',
      fingerprint: null
    };

    this.auth.login(payload).subscribe({
      next: () => {
        this.stopLoading();
        this.router.navigate(['/admin']);
      },
      error: (err) => {
        this.stopLoading('Invalid credentials. Please try again.');
        console.error('Login error:', err);
      }
    });
  }
}
