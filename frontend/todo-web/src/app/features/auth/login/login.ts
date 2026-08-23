import {
  ChangeDetectionStrategy,
  Component,
  inject,
  signal,
} from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';

import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-login',
  imports: [
    ReactiveFormsModule,
  ],
  templateUrl: './login.html',
  styleUrl: './login.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Login {
  private readonly formBuilder =
    inject(FormBuilder);

  private readonly authService =
    inject(AuthService);

  private readonly router =
    inject(Router);

  protected readonly submitting =
    signal(false);

  protected readonly errorMessage =
    signal<string | null>(null);

  protected readonly loginForm =
    this.formBuilder.nonNullable.group({
      email: [
        '',
        [
          Validators.required,
          Validators.email,
        ],
      ],

      password: [
        '',
        [
          Validators.required,
        ],
      ],
    });

  protected submit(): void {
    this.errorMessage.set(null);

    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.submitting.set(true);

    this.authService
      .login(this.loginForm.getRawValue())
      .subscribe({
        next: () => {
          this.submitting.set(false);

          void this.router.navigate(
            ['/todos'],
          );
        },

        error: (error: HttpErrorResponse) => {
          this.submitting.set(false);

          this.errorMessage.set(
            this.getLoginErrorMessage(error),
          );
        },
      });
  }

  private getLoginErrorMessage(
    error: HttpErrorResponse,
  ): string {
    if (error.status === 0) {
      return 'Unable to connect to the API. Please check that the backend is running.';
    }

    if (error.status === 401) {
      return 'Email or password is incorrect.';
    }

    return 'Unable to sign in. Please try again.';
  }
}