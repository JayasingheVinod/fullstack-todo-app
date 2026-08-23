# Fullstack Todo App

A full-stack Todo application built with Angular and ASP.NET Core.

The application allows an authenticated user to:

- View their Todo items
- Add new Todo items
- Delete Todo items
- Sign in using a predefined demo account
- Access only Todo items associated with the authenticated user

The project was built as a small full-stack coding exercise while still demonstrating clear application boundaries, automated testing, authentication, validation, and continuous integration.

---

# Running the Application

## Prerequisites

Install:

- .NET 10 SDK
- Node.js 22
- npm
- Git


```
No database installation is required.

Todo data is stored using the EF Core InMemory provider.
```
## 1. Run the Backend

From the repository root:

```bash
cd backend
dotnet restore TodoApp.sln
dotnet run --project src/TodoApp.Api/TodoApp.Api.csproj
```

The API runs at:

```text
http://localhost:5080
```

Keep this terminal running.

---

## 2. Run the Frontend

Open another terminal.

From the repository root:

```bash
cd frontend/todo-web
npm install
npm start
```

The Angular application runs at:

```text
http://localhost:4200
```

Open this URL in a browser:

```text
http://localhost:4200
```

---

# Demo Account

Use the following credentials:

```text
Email:
demo@todo.local

Password:
Todo123!
```

The account is intentionally predefined for this coding exercise.

There is no registration workflow.

---

# Testing

## Backend Tests

From:

```text
backend/
```

run:

```bash
dotnet test TodoApp.sln
```

---

## Frontend Tests

From:

```text
frontend/todo-web/
```

run:

```bash
npm test -- --watch=false
```
---