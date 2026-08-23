import { Routes } from '@angular/router';

import {
  authGuard,
  guestGuard,
} from './core/auth/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    canActivate: [
      guestGuard,
    ],
    loadComponent: () =>
      import(
        './features/auth/login/login'
      ).then(
        (module) => module.Login,
      ),
    title: 'Sign in | Todo App',
  },

  {
    path: 'todos',
    canActivate: [
      authGuard,
    ],
    loadComponent: () =>
      import(
        './features/todos/todos'
      ).then(
        (module) => module.Todos,
      ),
    title: 'My Todos | Todo App',
  },

  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'todos',
  },

  {
    path: '**',
    redirectTo: 'todos',
  },
];