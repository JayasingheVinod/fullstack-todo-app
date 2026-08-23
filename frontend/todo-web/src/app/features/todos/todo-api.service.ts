import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  CreateTodoRequest,
  Todo,
} from './todo.models';

@Injectable({
  providedIn: 'root',
})
export class TodoApiService {
  private readonly http = inject(HttpClient);

  private readonly todosUrl =
    `${environment.apiUrl}/todos`;

  getTodos(): Observable<Todo[]> {
    return this.http.get<Todo[]>(
      this.todosUrl,
    );
  }

  createTodo(
    request: CreateTodoRequest,
  ): Observable<Todo> {
    return this.http.post<Todo>(
      this.todosUrl,
      request,
    );
  }

  deleteTodo(id: string): Observable<void> {
    return this.http.delete<void>(
      `${this.todosUrl}/${id}`,
    );
  }
}