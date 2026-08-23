export interface Todo {
  id: string;
  title: string;
  createdAtUtc: string;
}

export interface CreateTodoRequest {
  title: string;
}

export interface ApiProblemDetails {
  title?: string;
  detail?: string;
  errors?: Record<string, string[]>;
}