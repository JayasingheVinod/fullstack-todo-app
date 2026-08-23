import { HttpErrorResponse } from '@angular/common/http';
import {
  ComponentFixture,
  TestBed,
} from '@angular/core/testing';
import { Router } from '@angular/router';
import {
  Observable,
  of,
  throwError,
} from 'rxjs';
import { vi } from 'vitest';

import {
  LoginResponse,
} from '../../../core/auth/auth.models';
import { AuthService } from '../../../core/auth/auth.service';
import { Login } from './login';

describe('Login', () => {
  let fixture: ComponentFixture<Login>;

  let loginResult:
    Observable<LoginResponse>;

  const authService = {
    login: vi.fn(
      () => loginResult,
    ),
  };

  const router = {
    navigate: vi.fn(
      () => Promise.resolve(true),
    ),
  };

  beforeEach(async () => {
    loginResult = of({
      accessToken: 'test-token',
      expiresAtUtc: new Date(
        Date.now() + 60 * 60 * 1000,
      ).toISOString(),
      email: 'demo@todo.local',
    });

    authService.login.mockClear();
    router.navigate.mockClear();

    await TestBed.configureTestingModule({
      imports: [
        Login,
      ],
      providers: [
        {
          provide: AuthService,
          useValue: authService,
        },
        {
          provide: Router,
          useValue: router,
        },
      ],
    }).compileComponents();

    fixture =
      TestBed.createComponent(Login);

    fixture.detectChanges();
  });

  it('shows validation errors when the form is submitted empty', () => {
    submitForm();

    fixture.detectChanges();

    expect(
      fixture.nativeElement.textContent,
    ).toContain('Email is required.');

    expect(
      fixture.nativeElement.textContent,
    ).toContain('Password is required.');

    expect(
      authService.login,
    ).not.toHaveBeenCalled();
  });

  it('logs in and navigates to todos with valid credentials', () => {
    setInputValue(
      '#email',
      'demo@todo.local',
    );

    setInputValue(
      '#password',
      'Todo123!',
    );

    submitForm();

    fixture.detectChanges();

    expect(
      authService.login,
    ).toHaveBeenCalledWith({
      email: 'demo@todo.local',
      password: 'Todo123!',
    });

    expect(
      router.navigate,
    ).toHaveBeenCalledWith(
      ['/todos'],
    );
  });

  it('shows an authentication error for invalid credentials', () => {
    loginResult = throwError(
      () =>
        new HttpErrorResponse({
          status: 401,
          statusText: 'Unauthorized',
        }),
    );

    setInputValue(
      '#email',
      'demo@todo.local',
    );

    setInputValue(
      '#password',
      'wrong-password',
    );

    submitForm();

    fixture.detectChanges();

    expect(
      fixture.nativeElement.textContent,
    ).toContain(
      'Email or password is incorrect.',
    );
  });

  it('shows a connection error when the API cannot be reached', () => {
    loginResult = throwError(
      () =>
        new HttpErrorResponse({
          status: 0,
          statusText: 'Unknown Error',
        }),
    );

    setInputValue(
      '#email',
      'demo@todo.local',
    );

    setInputValue(
      '#password',
      'Todo123!',
    );

    submitForm();

    fixture.detectChanges();

    expect(
      fixture.nativeElement.textContent,
    ).toContain(
      'Unable to connect to the API.',
    );
  });

  function setInputValue(
    selector: string,
    value: string,
  ): void {
    const input =
      fixture.nativeElement.querySelector(
        selector,
      ) as HTMLInputElement;

    input.value = value;

    input.dispatchEvent(
      new Event('input'),
    );

    fixture.detectChanges();
  }

  function submitForm(): void {
    const form =
      fixture.nativeElement.querySelector(
        'form',
      ) as HTMLFormElement;

    form.dispatchEvent(
      new Event('submit'),
    );
  }
});