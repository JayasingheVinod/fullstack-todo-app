import {
  ChangeDetectionStrategy,
  Component,
  inject,
} from '@angular/core';
import { Router } from '@angular/router';

import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-todos',
  templateUrl: './todos.html',
  styleUrl: './todos.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Todos {
  protected readonly authService =
    inject(AuthService);

  private readonly router =
    inject(Router);

  protected logout(): void {
    this.authService.logout();

    void this.router.navigate(
      ['/login'],
    );
  }
}
