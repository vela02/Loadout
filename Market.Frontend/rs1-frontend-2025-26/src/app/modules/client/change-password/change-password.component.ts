import { Component, inject } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroupDirective,
  ValidationErrors,
  ValidatorFn,
  Validators
} from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { UsersApiService } from '../../../api-services/users/users-api.service';
import { AuthFacadeService } from '../../../core/services/auth/auth-facade.service';
import { ToasterService } from '../../../core/services/toaster.service';

/** New password must match the confirmation and differ from the current one */
const passwordsValidator: ValidatorFn = (group: AbstractControl): ValidationErrors | null => {
  const oldPassword = group.get('oldPassword')?.value;
  const newPassword = group.get('newPassword')?.value;
  const confirmPassword = group.get('confirmPassword')?.value;

  const errors: ValidationErrors = {};
  if (newPassword && confirmPassword && newPassword !== confirmPassword) {
    errors['mismatch'] = true;
  }
  if (oldPassword && newPassword && oldPassword === newPassword) {
    errors['sameAsOld'] = true;
  }
  return Object.keys(errors).length ? errors : null;
};

@Component({
  selector: 'app-change-password',
  standalone: false,
  templateUrl: './change-password.component.html',
  styleUrl: './change-password.component.scss'
})
export class ChangePasswordComponent {
  private fb = inject(FormBuilder);
  private api = inject(UsersApiService);
  private auth = inject(AuthFacadeService);
  private toaster = inject(ToasterService);
  private translate = inject(TranslateService);

  form = this.fb.nonNullable.group(
    {
      oldPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required]
    },
    { validators: passwordsValidator }
  );

  isLoading = false;
  errorMessage = '';
  hideOld = true;
  hideNew = true;
  hideConfirm = true;

  onSubmit(formDirective: FormGroupDirective): void {
    if (this.form.invalid || this.isLoading) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    const { oldPassword, newPassword } = this.form.getRawValue();

    this.api.changePassword({
      oldPassword,
      newPassword,
      currentRefreshToken: this.auth.getRefreshToken()
    }).subscribe({
      next: () => {
        this.isLoading = false;
        this.toaster.success(this.translate.instant('CLIENT.PASSWORD.SUCCESS'));
        // resetForm also clears the "submitted" state, so no errors show on the empty form
        formDirective.resetForm();
        this.form.reset();
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = 'CLIENT.PASSWORD.FAILED';
        console.error('Change password error:', err);
      }
    });
  }
}
