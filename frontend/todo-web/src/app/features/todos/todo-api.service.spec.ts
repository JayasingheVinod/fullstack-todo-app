import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { firstValueFrom } from 'rxjs';

import { environment } from '../../../environments/environment';
import { TodoApiService } from './todo-api.service';
import { Todo } from './todo.models';

describe('TodoApiService', () => {
  let service: TodoApiService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        TodoApiService,
        provideHttpClient(),
        provideHttpClientTesting(),
      ],
    });

    service = TestBed.inject(TodoApiService);
    httpTesting = TestBed.inject(
      HttpTestingController,
    );
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('gets todos from the API', async () => {
    const expectedTodos: Todo[] = [
      {
        id: 'todo-1',
        title: 'Prepare interview',
        createdAtUtc: '2026-08-24T00:00:00Z',
      },
    ];

    const responsePromise =
      firstValueFrom(service.getTodos());

    const request = httpTesting.expectOne(
      `${environment.apiUrl}/todos`,
    );

    expect(request.request.method).toBe('GET');

    request.flush(expectedTodos);

    const todos = await responsePromise;

    expect(todos).toEqual(expectedTodos);
  });

  it('creates a todo', async () => {
    const createdTodo: Todo = {
      id: 'todo-1',
      title: 'Prepare interview',
      createdAtUtc: '2026-08-24T00:00:00Z',
    };

    const responsePromise =
      firstValueFrom(
        service.createTodo({
          title: 'Prepare interview',
        }),
      );

    const request = httpTesting.expectOne(
      `${environment.apiUrl}/todos`,
    );

    expect(request.request.method).toBe('POST');

    expect(request.request.body).toEqual({
      title: 'Prepare interview',
    });

    request.flush(createdTodo);

    const todo = await responsePromise;

    expect(todo).toEqual(createdTodo);
  });

  it('deletes a todo', async () => {
    const responsePromise =
      firstValueFrom(
        service.deleteTodo('todo-1'),
      );

    const request = httpTesting.expectOne(
      `${environment.apiUrl}/todos/todo-1`,
    );

    expect(request.request.method).toBe('DELETE');

    request.flush(null);

    await responsePromise;
  });
});