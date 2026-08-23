import {
  HttpClient,
  provideHttpClient,
  withInterceptors,
} from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { firstValueFrom } from 'rxjs';

import { environment } from '../../../environments/environment';
import { authInterceptor } from '../../core/auth/auth.interceptor';
import { AuthService } from '../../core/auth/auth.service';

describe('authInterceptor', () => {
  let http: HttpClient;
  let httpTesting: HttpTestingController;

  let accessToken: string | null;

  const authService = {
    getAccessToken: () => accessToken,
  };

  beforeEach(() => {
    accessToken = null;

    TestBed.configureTestingModule({
      providers: [
        {
          provide: AuthService,
          useValue: authService,
        },

        provideHttpClient(
          withInterceptors([
            authInterceptor,
          ]),
        ),

        provideHttpClientTesting(),
      ],
    });

    http = TestBed.inject(HttpClient);

    httpTesting = TestBed.inject(
      HttpTestingController,
    );
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('adds the bearer token to Todo API requests', async () => {
    accessToken = 'test-token';

    const responsePromise =
      firstValueFrom(
        http.get(
          `${environment.apiUrl}/todos`,
        ),
      );

    const request = httpTesting.expectOne(
      `${environment.apiUrl}/todos`,
    );

    expect(
      request.request.headers.get(
        'Authorization',
      ),
    ).toBe('Bearer test-token');

    request.flush([]);

    await responsePromise;
  });

  it('does not add authorization when there is no token', async () => {
    const responsePromise =
      firstValueFrom(
        http.get(
          `${environment.apiUrl}/todos`,
        ),
      );

    const request = httpTesting.expectOne(
      `${environment.apiUrl}/todos`,
    );

    expect(
      request.request.headers.has(
        'Authorization',
      ),
    ).toBe(false);

    request.flush([]);

    await responsePromise;
  });

  it('does not send the token to another origin', async () => {
    accessToken = 'test-token';

    const responsePromise =
      firstValueFrom(
        http.get(
          'https://example.com/data',
        ),
      );

    const request = httpTesting.expectOne(
      'https://example.com/data',
    );

    expect(
      request.request.headers.has(
        'Authorization',
      ),
    ).toBe(false);

    request.flush({});

    await responsePromise;
  });
});