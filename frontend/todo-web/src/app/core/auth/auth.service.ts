import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  AuthSession,
  LoginRequest,
  LoginResponse,
} from './auth.models';

const AUTH_STORAGE_KEY = 'todo-app.auth-session';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly http = inject(HttpClient);

  private readonly sessionState = signal<AuthSession | null>(
    this.loadSession(),
  );

  readonly session = this.sessionState.asReadonly();

  readonly currentUserEmail = computed(
    () => this.sessionState()?.email ?? null,
  );

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(
        `${environment.apiUrl}/auth/login`,
        request,
      )
      .pipe(
        tap((response) => {
          this.saveSession({
            accessToken: response.accessToken,
            expiresAtUtc: response.expiresAtUtc,
            email: response.email,
          });
        }),
      );
  }

  logout(): void {
    this.clearSession();
  }

  getAccessToken(): string | null {
    const session = this.sessionState();

    if (!this.isSessionValid(session)) {
      if (session) {
        this.clearSession();
      }

      return null;
    }

    return session.accessToken;
  }

  hasValidSession(): boolean {
    return this.getAccessToken() !== null;
  }

  private saveSession(session: AuthSession): void {
    sessionStorage.setItem(
      AUTH_STORAGE_KEY,
      JSON.stringify(session),
    );

    this.sessionState.set(session);
  }

  private loadSession(): AuthSession | null {
    const storedSession =
      sessionStorage.getItem(AUTH_STORAGE_KEY);

    if (!storedSession) {
      return null;
    }

    try {
      const session =
        JSON.parse(storedSession) as AuthSession;

      if (!this.isSessionValid(session)) {
        sessionStorage.removeItem(AUTH_STORAGE_KEY);
        return null;
      }

      return session;
    } catch {
      sessionStorage.removeItem(AUTH_STORAGE_KEY);
      return null;
    }
  }

  private clearSession(): void {
    sessionStorage.removeItem(AUTH_STORAGE_KEY);
    this.sessionState.set(null);
  }

  private isSessionValid(
    session: AuthSession | null,
  ): session is AuthSession {
    if (
      !session ||
      !session.accessToken ||
      !session.expiresAtUtc ||
      !session.email
    ) {
      return false;
    }

    return (
      Date.parse(session.expiresAtUtc) >
      Date.now()
    );
  }
}