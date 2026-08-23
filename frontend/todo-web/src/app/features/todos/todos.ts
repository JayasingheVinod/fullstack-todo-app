import { HttpErrorResponse } from '@angular/common/http';
import {
  ChangeDetectionStrategy,
  Component,
  inject,
  OnInit,
  signal,
} from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
} from '@angular/forms';
import { DatePipe } from '@angular/common';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';

import { AuthService } from '../../core/auth/auth.service';
import { TodoApiService } from './todo-api.service';
import {
  ApiProblemDetails,
  Todo,
} from './todo.models';

const TODO_TITLE_MAX_LENGTH = 200;

const todoTitleValidator: ValidatorFn = (
  control: AbstractControl,
): ValidationErrors | null => {
  const value =
    typeof control.value === 'string'
      ? control.value.trim()
      : '';

  if (!value) {
    return {
      required: true,
    };
  }

  if (value.length > TODO_TITLE_MAX_LENGTH) {
    return {
      maxlength: {
        requiredLength: TODO_TITLE_MAX_LENGTH,
        actualLength: value.length,
      },
    };
  }

  return null;
};

@Component({
  selector: 'app-todos',
  imports: [
    ReactiveFormsModule,
    DatePipe,
  ],
  templateUrl: './todos.html',
  styleUrl: './todos.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Todos implements OnInit {
  protected readonly authService = inject(AuthService);
  private readonly todoApi = inject(TodoApiService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly router = inject(Router);
  protected readonly todos = signal<Todo[]>([]);
  protected readonly loading = signal(true);
  protected readonly adding = signal(false);
  protected readonly deletingTodoId = signal<string | null>(null);
  protected readonly loadError = signal<string | null>(null);
  protected readonly actionError = signal<string | null>(null);
  protected readonly todoForm =
    this.formBuilder.nonNullable.group({
      title: [
        '',
        [
          todoTitleValidator,
        ],
      ],
    });

  ngOnInit(): void {
    this.loadTodos();
  }

  protected loadTodos(): void {
    this.loading.set(true);
    this.loadError.set(null);

    this.todoApi
      .getTodos()
      .pipe(
        finalize(() => {
          this.loading.set(false);
        }),
      )
      .subscribe({
        next: (todos) => {
          this.todos.set(todos);
        },

        error: (error: unknown) => {
          if (this.handleUnauthorized(error)) {
            return;
          }

          this.loadError.set(
            this.getErrorMessage(
              error,
              'Unable to load your todos. Please try again.',
            ),
          );
        },
      });
  }

  protected addTodo(): void {
    this.actionError.set(null);

    if (this.todoForm.invalid) {
      this.todoForm.markAllAsTouched();
      return;
    }

    const title =
      this.todoForm.controls.title.value.trim();

    this.adding.set(true);

    this.todoApi
      .createTodo({
        title,
      })
      .pipe(
        finalize(() => {
          this.adding.set(false);
        }),
      )
      .subscribe({
        next: (todo) => {
          this.todos.update(
            (todos) => [
              todo,
              ...todos,
            ],
          );

          this.todoForm.reset();
        },

        error: (error: unknown) => {
          if (this.handleUnauthorized(error)) {
            return;
          }

          this.actionError.set(
            this.getErrorMessage(
              error,
              'Unable to add the todo. Please try again.',
            ),
          );
        },
      });
  }

  protected deleteTodo(todo: Todo): void {
    if (this.deletingTodoId()) {
      return;
    }

    this.actionError.set(null);
    this.deletingTodoId.set(todo.id);

    this.todoApi
      .deleteTodo(todo.id)
      .pipe(
        finalize(() => {
          this.deletingTodoId.set(null);
        }),
      )
      .subscribe({
        next: () => {
          this.todos.update(
            (todos) =>
              todos.filter(
                (item) =>
                  item.id !== todo.id,
              ),
          );
        },

        error: (error: unknown) => {
          if (this.handleUnauthorized(error)) {
            return;
          }

          this.actionError.set(
            this.getErrorMessage(
              error,
              'Unable to delete the todo. Please try again.',
            ),
          );
        },
      });
  }

  protected logout(): void {
    this.authService.logout();

    void this.router.navigate(
      ['/login'],
    );
  }

  private handleUnauthorized(
    error: unknown,
  ): boolean {
    if (
      error instanceof HttpErrorResponse &&
      error.status === 401
    ) {
      this.authService.logout();

      void this.router.navigate(
        ['/login'],
      );

      return true;
    }

    return false;
  }

  private getErrorMessage(
    error: unknown,
    fallbackMessage: string,
  ): string {
    if (!(error instanceof HttpErrorResponse)) {
      return fallbackMessage;
    }

    if (error.status === 0) {
      return 'Unable to connect to the API. Please check that the backend is running.';
    }

    const problemDetails =
      error.error as ApiProblemDetails | null;

    const titleErrors =
      problemDetails?.errors?.['Title'];

    if (
      titleErrors &&
      titleErrors.length > 0
    ) {
      return titleErrors[0];
    }

    if (problemDetails?.detail) {
      return problemDetails.detail;
    }

    return fallbackMessage;
  }
}