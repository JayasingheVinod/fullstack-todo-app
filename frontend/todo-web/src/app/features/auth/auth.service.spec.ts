import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { firstValueFrom } from 'rxjs';

import { environment } from '../../../environments/environment';
import { AuthService } from '../../core/auth/auth.service';

const STORAGE_KEY = 'todo-app.auth-session';

describe('AuthService', () => {
  let service: AuthService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    sessionStorage.clear();

    TestBed.configureTestingModule({
      providers: [
        AuthService,
        provideHttpClient(),
        provideHttpClientTesting(),
      ],
    });

    service = TestBed.inject(AuthService);

    httpTesting = TestBed.inject(
      HttpTestingController,
    );
  });

  afterEach(() => {
    httpTesting.verify();
    sessionStorage.clear();
  });

  it('logs in and stores the authenticated session', async () => {
    const loginPromise =
      firstValueFrom(
        service.login({
          email: 'demo@todo.local',
          password: 'Todo123!',
        }),
      );

    const request = httpTesting.expectOne(
      `${environment.apiUrl}/auth/login`,
    );

    expect(request.request.method).toBe('POST');

    expect(request.request.body).toEqual({
      email: 'demo@todo.local',
      password: 'Todo123!',
    });

    request.flush({
      accessToken: 'test-access-token',
      expiresAtUtc: futureDate(),
      email: 'demo@todo.local',
    });

    await loginPromise;

    expect(service.getAccessToken()).toBe(
      'test-access-token',
    );

    expect(
      service.currentUserEmail(),
    ).toBe('demo@todo.local');

    expect(
      sessionStorage.getItem(STORAGE_KEY),
    ).not.toBeNull();
  });

  it('restores a valid session from session storage', () => {
    sessionStorage.setItem(
      STORAGE_KEY,
      JSON.stringify({
        accessToken: 'stored-token',
        expiresAtUtc: futureDate(),
        email: 'demo@todo.local',
      }),
    );

    TestBed.resetTestingModule();

    TestBed.configureTestingModule({
      providers: [
        AuthService,
        provideHttpClient(),
        provideHttpClientTesting(),
      ],
    });

    const restoredService =
      TestBed.inject(AuthService);

    expect(
      restoredService.getAccessToken(),
    ).toBe('stored-token');

    expect(
      restoredService.currentUserEmail(),
    ).toBe('demo@todo.local');
  });

  it('removes an expired stored session', () => {
    sessionStorage.setItem(
      STORAGE_KEY,
      JSON.stringify({
        accessToken: 'expired-token',
        expiresAtUtc: pastDate(),
        email: 'demo@todo.local',
      }),
    );

    TestBed.resetTestingModule();

    TestBed.configureTestingModule({
      providers: [
        AuthService,
        provideHttpClient(),
        provideHttpClientTesting(),
      ],
    });

    const restoredService =
      TestBed.inject(AuthService);

    expect(
      restoredService.hasValidSession(),
    ).toBe(false);

    expect(
      sessionStorage.getItem(STORAGE_KEY),
    ).toBeNull();
  });

  it('clears the session when logging out', () => {
    sessionStorage.setItem(
      STORAGE_KEY,
      JSON.stringify({
        accessToken: 'stored-token',
        expiresAtUtc: futureDate(),
        email: 'demo@todo.local',
      }),
    );

    TestBed.resetTestingModule();

    TestBed.configureTestingModule({
      providers: [
        AuthService,
        provideHttpClient(),
        provideHttpClientTesting(),
      ],
    });

    const authenticatedService =
      TestBed.inject(AuthService);

    authenticatedService.logout();

    expect(
      authenticatedService.hasValidSession(),
    ).toBe(false);

    expect(
      authenticatedService.currentUserEmail(),
    ).toBeNull();

    expect(
      sessionStorage.getItem(STORAGE_KEY),
    ).toBeNull();
  });
});

function futureDate(): string {
  return new Date(
    Date.now() + 60 * 60 * 1000,
  ).toISOString();
}

function pastDate(): string {
  return new Date(
    Date.now() - 60 * 60 * 1000,
  ).toISOString();
}