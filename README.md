
# HelpDeskManagement

Help Desk Ticket Management System built using ASP.NET Core Web API, ASP.NET Core MVC, Entity Framework Core, SQL Server, xUnit, Moq and GitHub.

## Solution Structure

```
HelpDeskManagement.sln
├── HelpDesk.Api/          # ASP.NET Core Web API (backend, EF Core, Repository Pattern)
├── HelpDesk.Mvc/          # ASP.NET Core MVC front end that consumes the API
├── HelpDesk.Tests/        # xUnit + Moq test project (repository & controller tests)
├── README.md
└── .gitignore
```

## Ticket Model

| Field        | Type     | Notes                              |
|--------------|----------|-------------------------------------|
| Id           | int      | Primary key                        |
| Title        | string   | Required                           |
| Description  | string   |                                     |
| Priority     | string   | `Low`, `Medium`, `High`             |
| Status       | string   | `Open`, `In Progress`, `Closed`     |
| RaisedBy     | string   | Required                           |
| CreatedDate  | DateTime | Set automatically on creation       |

## Part 1 — HelpDesk.Api

* Configured with SQL Server + Entity Framework Core (`HelpDeskDbContext`).
* Repository Pattern implemented in `Repositories/`:
  * `ITicketRepository` — interface
  * `TicketRepository` — implementation, all operations are async
* `TicketsController` exposes REST endpoints:

| Verb   | Route                        | Description                    |
|--------|-------------------------------|---------------------------------|
| GET    | `/api/tickets`                | Get all tickets                |
| GET    | `/api/tickets/{id}`           | Get a ticket by id              |
| GET    | `/api/tickets/status/{status}`| Get tickets filtered by status  |
| POST   | `/api/tickets`                | Create a new ticket             |
| PUT    | `/api/tickets/{id}`           | Update an existing ticket       |
| DELETE | `/api/tickets/{id}`           | Delete a ticket                 |

### Setup

1. Update the connection string in `HelpDesk.Api/appsettings.json`.
2. From the `HelpDesk.Api` folder, run the EF Core migration and update the database:

   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

3. Run the API:

   ```bash
   dotnet run --project HelpDesk.Api
   ```

4. Browse Swagger UI at `https://localhost:7225/swagger`.

## Part 2 — HelpDesk.Mvc

A simple ASP.NET Core MVC app (`TicketController` + Razor views under `Views/Ticket`) that consumes `HelpDesk.Api` over HTTP for full CRUD (list, create, edit, details, delete). Configure the API base address in `HelpDesk.Mvc/appsettings.json` under `ApiSettings:BaseUrl`.

```bash
dotnet run --project HelpDesk.Mvc
```

## Part 3 — HelpDesk.Tests

xUnit test project covering:
* `TicketRepositoryTests` — repository logic against EF Core's In-Memory provider.
* `TicketsControllerTests` — controller behavior using Moq to mock `ITicketRepository`.

```bash
dotnet test HelpDesk.Tests
```

## Coding Guidelines Followed

* Proper naming conventions
* Repository Pattern
* Asynchronous methods throughout
* Exception handling in controller actions (try/catch + appropriate status codes)
* Meaningful, incremental commit messages when pushed to GitHub
* Clean code and consistent indentation

## Restoring & Building

This solution targets **.NET 8**. From the repository root:

```bash
dotnet restore
dotnet build
```
