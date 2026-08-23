import { HttpErrorResponse } from '@angular/common/http';
import {
  ComponentFixture,
  TestBed,
} from '@angular/core/testing';
import { signal } from '@angular/core';
import { Router } from '@angular/router';
import {
  Observable,
  of,
  throwError,
} from 'rxjs';
import { vi } from 'vitest';

import { AuthService } from '../../core/auth/auth.service';
import { TodoApiService } from './todo-api.service';
import {
  CreateTodoRequest,
  Todo,
} from './todo.models';
import { Todos } from './todos';

describe('Todos', () => {
  let fixture: ComponentFixture<Todos>;

  let getTodosResult:
    Observable<Todo[]>;

  let createTodoResult:
    Observable<Todo>;

  let deleteTodoResult:
    Observable<void>;

  const currentUserEmail =
    signal<string | null>(
      'demo@todo.local',
    );

  const authService = {
    currentUserEmail,
    logout: vi.fn(),
  };

  const todoApi = {
    getTodos: vi.fn(
      () => getTodosResult,
    ),

    createTodo: vi.fn(
      (_request: CreateTodoRequest) =>
        createTodoResult,
    ),

    deleteTodo: vi.fn(
      (_id: string) =>
        deleteTodoResult,
    ),
  };

  const router = {
    navigate: vi.fn(
      () => Promise.resolve(true),
    ),
  };

  beforeEach(async () => {
    getTodosResult = of([]);

    createTodoResult = of({
      id: 'todo-created',
      title: 'New todo',
      createdAtUtc:
        '2026-08-24T00:00:00Z',
    });

    deleteTodoResult = of(undefined);

    authService.logout.mockClear();

    todoApi.getTodos.mockClear();
    todoApi.createTodo.mockClear();
    todoApi.deleteTodo.mockClear();

    router.navigate.mockClear();

    await TestBed.configureTestingModule({
      imports: [
        Todos,
      ],
      providers: [
        {
          provide: AuthService,
          useValue: authService,
        },
        {
          provide: TodoApiService,
          useValue: todoApi,
        },
        {
          provide: Router,
          useValue: router,
        },
      ],
    }).compileComponents();
  });

  it('loads and displays todos', () => {
    getTodosResult = of([
      {
        id: 'todo-1',
        title: 'Prepare interview',
        createdAtUtc:
          '2026-08-24T00:00:00Z',
      },
    ]);

    createComponent();

    expect(
      todoApi.getTodos,
    ).toHaveBeenCalled();

    expect(
      fixture.nativeElement.textContent,
    ).toContain('Prepare interview');

    const itemCount =
      fixture.nativeElement.querySelector(
        '.list-header p',
      ) as HTMLParagraphElement;

    expect(
      itemCount.textContent?.trim()
        .replace(/\s+/g, ' '),
    ).toBe('1 item');
  });

  it('shows the empty state when there are no todos', () => {
    createComponent();

    expect(
      fixture.nativeElement.textContent,
    ).toContain('Nothing here yet');

    expect(
      fixture.nativeElement.textContent,
    ).toContain(
      'Add your first todo above.',
    );
  });

  it('does not create a blank todo', () => {
    createComponent();

    setTodoTitle('   ');

    submitTodoForm();

    fixture.detectChanges();

    expect(
      todoApi.createTodo,
    ).not.toHaveBeenCalled();

    expect(
      fixture.nativeElement.textContent,
    ).toContain(
      'Enter a todo title.',
    );
  });

  it('creates a todo and displays it', () => {
    createTodoResult = of({
      id: 'todo-2',
      title: 'Review Angular',
      createdAtUtc:
        '2026-08-24T01:00:00Z',
    });

    createComponent();

    setTodoTitle(
      'Review Angular',
    );

    submitTodoForm();

    fixture.detectChanges();

    expect(
      todoApi.createTodo,
    ).toHaveBeenCalledWith({
      title: 'Review Angular',
    });

    expect(
      fixture.nativeElement.textContent,
    ).toContain('Review Angular');

    const input =
      fixture.nativeElement.querySelector(
        '#todo-title',
      ) as HTMLInputElement;

    expect(input.value).toBe('');
  });

  it('trims the todo title before creating it', () => {
    createTodoResult = of({
      id: 'todo-2',
      title: 'Buy groceries',
      createdAtUtc:
        '2026-08-24T01:00:00Z',
    });

    createComponent();

    setTodoTitle(
      '   Buy groceries   ',
    );

    submitTodoForm();

    expect(
      todoApi.createTodo,
    ).toHaveBeenCalledWith({
      title: 'Buy groceries',
    });
  });

  it('deletes a todo after the API succeeds', () => {
    getTodosResult = of([
      {
        id: 'todo-1',
        title: 'Delete me',
        createdAtUtc:
          '2026-08-24T00:00:00Z',
      },
    ]);

    createComponent();

    const deleteButton =
      fixture.nativeElement.querySelector(
        '.delete-button',
      ) as HTMLButtonElement;

    deleteButton.click();

    fixture.detectChanges();

    expect(
      todoApi.deleteTodo,
    ).toHaveBeenCalledWith(
      'todo-1',
    );

    expect(
      fixture.nativeElement.textContent,
    ).not.toContain('Delete me');
  });

  it('shows an error when todos cannot be loaded', () => {
    getTodosResult = throwError(
      () =>
        new HttpErrorResponse({
          status: 0,
          statusText: 'Unknown Error',
        }),
    );

    createComponent();

    expect(
      fixture.nativeElement.textContent,
    ).toContain(
      'Unable to connect to the API.',
    );

    expect(
      fixture.nativeElement.textContent,
    ).toContain('Try again');
  });

  it('logs out and redirects when the API returns unauthorized', () => {
    getTodosResult = throwError(
      () =>
        new HttpErrorResponse({
          status: 401,
          statusText: 'Unauthorized',
        }),
    );

    createComponent();

    expect(
      authService.logout,
    ).toHaveBeenCalled();

    expect(
      router.navigate,
    ).toHaveBeenCalledWith(
      ['/login'],
    );
  });

  function createComponent(): void {
    fixture =
      TestBed.createComponent(Todos);

    fixture.detectChanges();
  }

  function setTodoTitle(
    value: string,
  ): void {
    const input =
      fixture.nativeElement.querySelector(
        '#todo-title',
      ) as HTMLInputElement;

    input.value = value;

    input.dispatchEvent(
      new Event('input'),
    );

    fixture.detectChanges();
  }

  function submitTodoForm(): void {
    const form =
      fixture.nativeElement.querySelector(
        '.todo-form',
      ) as HTMLFormElement;

    form.dispatchEvent(
      new Event('submit'),
    );
  }
});
