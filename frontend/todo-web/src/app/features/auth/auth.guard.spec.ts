import { TestBed } from '@angular/core/testing';
import {
  provideRouter,
  Router,
  UrlTree,
} from '@angular/router';

import {
  authGuard,
  guestGuard,
} from '../../core/auth/auth.guard';
import { AuthService } from '../../core/auth/auth.service';

describe('authentication guards', () => {
  let hasValidSession: boolean;

  const authService = {
    hasValidSession: () => hasValidSession,
  };

  beforeEach(() => {
    hasValidSession = false;

    TestBed.configureTestingModule({
      providers: [
        provideRouter([]),

        {
          provide: AuthService,
          useValue: authService,
        },
      ],
    });
  });

  describe('authGuard', () => {
    it('allows navigation when authenticated', () => {
      hasValidSession = true;

      const result =
        TestBed.runInInjectionContext(
          () => authGuard(
            {} as never,
            {} as never,
          ),
        );

      expect(result).toBe(true);
    });

    it('redirects to login when unauthenticated', () => {
      hasValidSession = false;

      const router =
        TestBed.inject(Router);

      const result =
        TestBed.runInInjectionContext(
          () => authGuard(
            {} as never,
            {} as never,
          ),
        );

      expect(result).toBeInstanceOf(
        UrlTree,
      );

      expect(
        router.serializeUrl(
          result as UrlTree,
        ),
      ).toBe('/login');
    });
  });

  describe('guestGuard', () => {
    it('allows login when unauthenticated', () => {
      hasValidSession = false;

      const result =
        TestBed.runInInjectionContext(
          () => guestGuard(
            {} as never,
            {} as never,
          ),
        );

      expect(result).toBe(true);
    });

    it('redirects authenticated users to todos', () => {
      hasValidSession = true;

      const router =
        TestBed.inject(Router);

      const result =
        TestBed.runInInjectionContext(
          () => guestGuard(
            {} as never,
            {} as never,
          ),
        );

      expect(result).toBeInstanceOf(
        UrlTree,
      );

      expect(
        router.serializeUrl(
          result as UrlTree,
        ),
      ).toBe('/todos');
    });
  });
});