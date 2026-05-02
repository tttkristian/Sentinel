> **⚠️ Read `GENERAL_INSTRUCTIONS.md` first.** This document only adds Vantage-specific rules. All general rules (Clean Architecture, CQRS, code style, EF Core conventions, exception handling, testing, Git workflow, etc.) apply here unchanged.

---

## 1. Project Overview

**Sentinel** is a centralized call center that takes the number of businesses and gives them a virtual number to receive calls. This proyect will recieven the call and log the call information and leave it for the business.

### Goals
- One unified SQL Server database for all task data.
- One ASP.NET Core API exposing endpoints for every task module.
- Each app/module is organized as an independent feature area inside the same solution (modular monolith).
- Designed for **on-premise IIS deployment** initially, with a future migration path to Azure.
- Designe a communications platform using asterisk to handle the calls and send the information to the API.

### First Module: Sentinel


A call service that recieves calls and logs the call information for the business. The module will allow the user to:
1. Connect the phone number and give a virtual number to the business.
2. log the call information and leave it for the business.
3. redirect calls to the admin when the business is not available.
4. transcribe the call and send it via email to the business.

Subsequent modules will be added under the same Sentinel umbrella as new feature areas.

---

## 2. Solution Structure

```text
Sentinel/
├── src/
│   ├── Sentinel.Domain/
│   ├── Sentinel.Application/
│   ├── Sentinel.Infrastructure/
│   └── Sentinel.Api/
└── tests/
    ├── Sentinel.Domain.Tests/
    ├── Sentinel.Application.Tests/
    ├── Sentinel.Infrastructure.Tests/
    └── Sentinel.Api.Tests/
```

### Project & Namespace Conventions
- Root namespace: `Sentinel.<Layer>`
- Module-specific code lives under: `Sentinel.<Layer>.Features.<ModuleName>`
- Examples:
  - `Sentinel.Application.Features.Sentinel.<EntityName>.Commands`
  - `Sentinel.Infrastructure.Persistence.Configurations.Sentinel`
  - `Sentinel.Api.Endpoints.Sentinel`
### Modular Monolith Approach
- Each task app (Sentinel, future modules) is a **feature folder** inside the existing layers — not a separate solution or microservice.
- All modules share:
  - The same `SentinelDbContext` and database.
  - The same authentication and authorization.
  - The same global middleware, exception handling, and validation pipeline.
- This keeps deployment simple (one IIS site) while allowing each module to evolve independently.

---

## 3. Database Design

### Single Database, Multiple Schemas
- One SQL Server database: **`Sentinel`**.
- Each module gets its own **schema** to keep tables organized:
  - `Sentinel.<TableName>` — Sentinel module tables
  - `<module>.<TableName>` — future modules
  - `shared.<TableName>` — shared lookup or reference tables (if any)
- Configure the schema per entity in its `IEntityTypeConfiguration`:
  ```csharp
  builder.ToTable("<TableName>", schema: "sentinel");
  ```

### Table Naming
- The developer (you) defines table names manually based on business context.
- Follow the general column-type discipline from `GENERAL_INSTRUCTIONS.md` (no `nvarchar(max)`, reuse identical column definitions, etc.).

---

## 4. API Design

### Single API, Modular Endpoints
- One `Sentinel.Api` project exposes all endpoints.
- Use **Minimal APIs** organized by module and entity:
  ```text
  Sentinel.Api/
    Endpoints/
      Sentinel/
        <Entity>Endpoints.cs
      <FutureModule>/
        ...
  ```
- Endpoint route prefix follows the module name:
  - `/api/Sentinel/...`
  - `/api/<module>/...`

### Endpoint Registration Pattern
Each endpoint file exposes a static extension method to register its routes:

```csharp
public static class SampleEndpoints
{
    public static void MapSampleEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/Manager/<resource>")
                       .WithTags("MMD - <Resource>")
                       .RequireAuthorization();

        group.MapGet("/", GetAll);
        group.MapGet("/{id:guid}", GetById);
        group.MapPost("/", Upsert);
    }

    private static async Task<IResult> GetAll(IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new GetSamplesQuery(), ct);
        return Results.Ok(result);
    }
}
```

All endpoints are wired in `Program.cs`:

```csharp
app.MapSodaEndpoints();
// app.Map<FutureModule>Endpoints();
```

---

## 5. Deployment Target

- **Current:** On-premise Windows Server with IIS — single site, single application pool.
- **Future:** Azure (containerized) or heroku. Code should not contain hard dependencies that prevent this migration (e.g., avoid hardcoded paths, use abstractions for storage and external services).
- One `dotnet publish` output → one IIS site → one URL serving all modules.

---

## 6. Module Onboarding Checklist

When adding a new app/module to Sentinel, follow this checklist:

1. ✅ Create the module folder under `Sentinel.Domain/Entities/<Module>/`.
2. ✅ Define entities and their interfaces (`ISoftDelete`, `IActivatable` as needed).
3. ✅ Add `IEntityTypeConfiguration<T>` for each entity under `Sentinel.Infrastructure/Persistence/Configurations/<Module>/`.
4. ✅ Use the dedicated schema (`<module>.<TableName>`).
5. ✅ Add `DbSet<T>` to `ISentinelDbContext` and `SentinelDbContext`.
6. ✅ Create the CQRS folder structure under `Sentinel.Application/Features/<Module>/<Entity>/`.
7. ✅ Add `<Entity>Endpoints.cs` under `Sentinel.Api/Endpoints/<Module>/`.
8. ✅ Wire endpoints in `Program.cs`.
9. ✅ Create EF Core migration locally and verify.
10. ✅ Add handler tests using the InMemory database.

---

## 7. Database Connection

Sentinel uses the standard configuration pattern defined in `GENERAL_INSTRUCTIONS.md` §11.

### Connection String Key
The connection string is bound to the standard `ConnectionStrings:DefaultConnection` key.

### `appsettings.json` (committed — placeholder only)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  }
}
```

### Local Development — User Secrets
For local development, set the connection string using **.NET User Secrets** (never commit it):

```powershell
dotnet user-secrets init --project Sentinel.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\MSSQLLocalDB;Database=Sentinel_Dev;Trusted_Connection=True;TrustServerCertificate=True;" --project Sentinel.Api
```

The same User Secret is read by EF Core tooling when running migrations from the local environment.

### Deployed Environments — Environment Variables
For deployed environments (QA, Staging, Production), the connection string is injected as an environment variable that overrides the `appsettings.json` value at runtime:

```text
ConnectionStrings__DefaultConnection
```

The deployment pipeline injects the appropriate value per environment.

### EF Core Migration Commands
Migrations always read the connection string from the active configuration source (User Secrets locally, environment variables in deployed environments).

```powershell
# Create migration (LOCAL ONLY)
Add-Migration <DescriptiveName> -Project Manager.Infrastructure -StartupProject Manager.Api

# Apply migration to the currently configured database
Update-Database -Project Manager.Infrastructure -StartupProject Manager.Api
```

> Migration environment progression rules: see `GENERAL_INSTRUCTIONS.md` §16.
---

## 8. What This Document Does NOT Define

The following are intentionally left to the developer to define with full business context:

- ❌ Specific table names and column names.
- ❌ Specific entity properties and relationships.
- ❌ Specific business rules of each module (e.g., monthly audit constraints, scoring logic, validation rules).
- ❌ Authentication strategy (JWT, Windows Auth, etc.) — to be decided based on infrastructure.
- ❌ UI/desktop client implementation (ManagerManager is the backend only).

