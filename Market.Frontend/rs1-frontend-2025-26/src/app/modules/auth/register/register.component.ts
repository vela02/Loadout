import { Component, inject } from '@angular/core';
import { AbstractControl, FormBuilder, ValidationErrors, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { switchMap } from 'rxjs';
import { BaseComponent } from '../../../core/components/base-classes/base-component';
import { AuthApiService } from '../../../api-services/auth/auth-api.service';
import { AuthFacadeService } from '../../../core/services/auth/auth-facade.service';
import { CurrentUserService } from '../../../core/services/auth/current-user.service';

/** Form-level validator: password and confirmPassword must be equal */
function passwordsMatch(group: AbstractControl): ValidationErrors | null {
  const password = group.get('password')?.value;
  const confirm = group.get('confirmPassword')?.value;
  return password && confirm && password !== confirm ? { passwordMismatch: true } : null;
}

@Component({
  selector: 'app-register',
  standalone: false,
  templateUrl: './register.component.html',
  // shares the login page look, plus a few register-only styles
  styleUrls: ['../login/login.component.scss', './register.component.scss'],
})
export class RegisterComponent extends BaseComponent {
  private fb = inject(FormBuilder);
  private authApi = inject(AuthApiService);
  private auth = inject(AuthFacadeService);
  private router = inject(Router);
  private currentUser = inject(CurrentUserService);
  private translate = inject(TranslateService);

  hidePassword = true;

  currentLang: string = localStorage.getItem('language') || 'bs';

  languages = [
    { code: 'bs', name: 'Bosanski', flag: '🇧🇦' },
    { code: 'en', name: 'English', flag: '🇬🇧' }
  ];

  // same rules as RegisterCommandValidator.cs on the backend
  form = this.fb.group(
    {
      username: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]],
    },
    { validators: passwordsMatch }
  );

  get passwordMismatch(): boolean {
    const confirm = this.form.get('confirmPassword');
    return !!confirm?.touched && this.form.hasError('passwordMismatch');
  }

  onSubmit(): void {
    this.form.markAllAsTouched();
    if (this.form.invalid || this.isLoading) return;

    this.startLoading();

    const username = this.form.value.username!.trim();
    const email = this.form.value.email!.trim();
    const password = this.form.value.password!;

    // register, then log in the normal way so access + refresh tokens get stored
    this.authApi
      .register({ username, email, password })
      .pipe(switchMap(() => this.auth.login({ email, password, fingerprint: null })))
      .subscribe({
        next: () => {
          this.stopLoading();
          this.router.navigate([this.currentUser.getDefaultRoute()]);
        },
        error: (err) => {
          this.stopLoading(this.translate.instant('AUTH.REGISTER.FAILED'));
          console.error('Register error:', err);
        },
      });
  }

  switchLanguage(langCode: string): void {
    this.currentLang = langCode;
    this.translate.use(langCode);
    localStorage.setItem('language', langCode);
  }

  getCurrentLanguage() {
    return this.languages.find(lang => lang.code === this.currentLang);
  }
}
