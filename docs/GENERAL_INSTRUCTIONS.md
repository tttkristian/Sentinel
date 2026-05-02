# General Development Instructions

Base instructions for all .NET projects. These rules apply globally regardless of the specific project context. Project-specific documents may extend or override these where explicitly stated.

---

## 1. Technology Stack
- **.NET 10.0+** with C# 12+ features
- **EF Core** latest stable version
- **CQRS** pattern via **MediatR**
- **FluentValidation** for input validation
- **xUnit** for testing
- **Moq** for mocking dependencies
- **EF Core InMemory Database** for handler tests
- **Microsoft.Extensions.DependencyInjection** for DI
- **Microsoft.Extensions.Logging** for structured logging

---

## 2. Solution Architecture (Clean Architecture)

Every solution must follow these 4 layers with strict dependency direction:

```text
Api  →  Application  →  Domain
                ↑
        Infrastructure
```

- **Domain**: Entities, Value Objects, Domain Exceptions, Specifications, common property interfaces.
- **Application**: CQRS handlers, DTOs, Validators, Abstractions (including the `IDbContext` interface).
- **Infrastructure**: EF Core implementation, `DbContext`, Entity Configurations, External services.
- **Api**: Minimal APIs, endpoint mapping, middleware, authentication.

### Critical Dependency Rule
- **Application defines the `IDbContext` abstraction**. Infrastructure implements it.
- **Application NEVER references Infrastructure**. Infrastructure references Application.
- This keeps the domain/application layers testable without EF Core dependencies.

### SOLID & Clean Code
- All code must comply with **SOLID principles**.
- Avoid code duplication through **base classes and utility extensions**.
- Use **meaningful names that reflect domain concepts** (no abbreviations like `usr`, `prd`).
- Keep methods **focused and cohesive** — each method does one thing well.
- Implement **proper disposal patterns** (`IDisposable` / `IAsyncDisposable`) for resources holding unmanaged or scarce resources.

---

## 3. Design Patterns

### Primary Constructors (DI)
Use **primary constructor syntax** for dependency injection in all classes:

```csharp
public sealed class SampleHandler(ISampleService service, ILogger<SampleHandler> logger)
    : IRequestHandler<SampleCommand, ResultIdDto>
{
    // Use 'service' and 'logger' directly
}
```

### Interface Segregation
- Prefix all interfaces with **`I`** (e.g., `ISampleService`).
- Keep interfaces small and focused — split fat interfaces into smaller, role-based ones.

### Factory Pattern
- Use the **Factory pattern** for complex object creation (objects with conditional setup, multiple dependencies, or polymorphic instantiation).
- Factories live in the layer that owns the object being constructed.

### Specifications Pattern
- Use the **Specifications pattern** to encapsulate reusable query criteria.
- See **Section 7 — Entity Framework Core** for `ISoftDelete` and `IsActive` rules.

---

## 4. Dependency Injection
- Each layer must expose a static class named **`DependencyInjection`** at the project root.
- This class registers all services for that layer via extension methods on `IServiceCollection`.
- Register services with the **appropriate lifetime**:
  - `Singleton` — stateless services shared across the app.
  - `Scoped` — per-request services (default for handlers, repositories, DbContext).
  - `Transient` — lightweight, stateless, short-lived services.
- Use **constructor injection only** — no service locator anti-pattern.
- For non-primary-constructor classes, validate dependencies with `ArgumentNullException.ThrowIfNull(...)`.

```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        return services;
    }
}
```

---

## 5. CQRS Pattern Rules

### Folder Structure (per entity / aggregate)
```text
Application/
  Features/
    <ModuleName>/
      <EntityName>/
        Commands/
          UpsertXCommand.cs        // Command + Handler in same file
          UpsertXCommandValidator.cs
        Queries/
          GetXQuery.cs             // Query + Handler in same file
        Dtos/
          XDto.cs
  Core/
    Commands/                      // Cross-cutting commands shared across modules
    Queries/
```

### Naming Conventions
| Type | Pattern | Example |
|---|---|---|
| Command | `<Action><Entity>Command` | `UpsertXCommand` |
| Handler | `<Command>Handler` | `UpsertXCommandHandler` |
| Validator | `<Command>Validator` | `UpsertXCommandValidator` |
| Query | `Get<Entity(s)>Query` | `GetXsQuery`, `GetXByIdQuery` |
| DTO | `<Entity>Dto` or `Result<Type>Dto` | `XDto`, `ResultIdDto` |

### Rules
- Commands return `ResultIdDto` (containing the entity's `Guid`) unless explicitly different.
- Use `record` types for commands and queries.
- Command handlers implement `IRequestHandler<TCommand, TResponse>`.
- Use **direct DbContext properties** (`db.<Entities>`) — never `db.Set<TEntity>()`.
- Every command **must** have a corresponding `Validator` using FluentValidation.
- The `ValidationBehavior` MediatR pipeline runs validators automatically — never invoke them manually inside handlers.

---

## 6. Code Style

### Project-Level
- `<Nullable>enable</Nullable>` in every `.csproj`.
- `<ImplicitUsings>enable</ImplicitUsings>` in every `.csproj`.
- File-scoped namespaces.

### Class-Level
- Use `sealed` for all classes that aren't intended to be inherited.
- Use **primary constructors** for dependency injection.
- Use `required` keyword for mandatory entity/model properties.
- Use `readonly record struct` for simple, immutable DTOs (e.g., `ResultIdDto`).
- Use C# 12+ features (collection expressions `[]`, primary constructors, etc.) where they improve clarity.

### Method-Level
- **Always use early return** — avoid deep nesting. Return early when conditions aren't met.
- **Always use braces `{}` for `if` statements**, even when the body is a single line:
  ```csharp
  // ✅ CORRECT
  if (entity is null)
  {
      throw new NotFoundException(nameof(entity), id);
  }

  // ❌ INCORRECT
  if (entity is null) throw new NotFoundException(nameof(entity), id);
  ```

### Async / Await Patterns
- Use `async/await` for all I/O operations and long-running tasks.
- All async methods return `Task` or `Task<T>` — never `void` (except for event handlers).
- Never use `.Result` or `.Wait()`.
- Use `ConfigureAwait(false)` in **library code**; not required in API/handler code.
- **Always pass `CancellationToken`** to async methods.
- Parameter name: `ct` (preferred) or `cancellationToken`.
- Handle async exceptions properly — never silently swallow them.

---

## 7. Entity Framework Core

### DbContext Abstraction
- The `DbContext` is exposed via an **interface in the Application layer** (e.g., `I<Project>DbContext`).
- Infrastructure provides the concrete implementation.
- Handlers depend on the interface, never on the concrete `DbContext`.

### Entity Configurations
- Every entity **must** have its own `IEntityTypeConfiguration<TEntity>` class.
- Configurations live in `Infrastructure/Persistence/Configurations`.
- Apply via `modelBuilder.ApplyConfigurationsFromAssembly(...)` in `OnModelCreating`.

### Column Type Discipline (Performance)
- **NEVER use `nvarchar(max)`** unless absolutely necessary (large unstructured text).
- Always specify the appropriate SQL type and length:
  - Codes/IDs: `nvarchar(20)`, `nvarchar(50)`
  - Names: `nvarchar(100)`, `nvarchar(200)`
  - Descriptions: `nvarchar(500)` or `nvarchar(1000)`
  - Money: `decimal(18,2)`
  - Booleans, dates, ints: use the correct primitive type.
- **Reuse identical column definitions across entities** (same type + same length) to improve indexing and execution plan reuse.
- Create **ConfigurationExtensions** for repeated column patterns.

### Common Property Interfaces & Specifications
Common cross-cutting properties are defined as marker interfaces in the Domain layer:

| Interface | Purpose |
|---|---|
| `ISoftDelete` | Entity supports soft deletion (e.g., `IsDeleted` flag) |
| `IActivatable` | Entity supports active/inactive state (e.g., `IsActive` flag) |

#### Use the Specifications Pattern (NOT global query filters)
- **Do NOT apply soft delete or `IsActive` filters via `HasQueryFilter` in `OnModelCreating`.**
- Global query filters cause subtle bugs (forgotten `IgnoreQueryFilters()`, unexpected joins, performance traps).
- Instead, define **reusable specifications** that handlers explicitly apply when querying.

```csharp
// Domain/Specifications/NotDeletedSpecification.cs
public static class EntityFilters
{
    public static IQueryable<T> WhereNotDeleted<T>(this IQueryable<T> query) where T : ISoftDelete
    {
        return query.Where(x => !x.IsDeleted);
    }

    public static IQueryable<T> WhereActive<T>(this IQueryable<T> query) where T : IActivatable
    {
        return query.Where(x => x.IsActive);
    }
}
```

```csharp
// Usage in a handler — explicit and intentional
var items = await db.Items
    .WhereNotDeleted()
    .WhereActive()
    .ToListAsync(ct);

// When you need ALL records (including deleted), simply omit the specification
var allItems = await db.Items.ToListAsync(ct);
```

This makes filter behavior **explicit at the call site** and removes the need for `IgnoreQueryFilters()`.

### IDs and Timestamps
- Use `Guid.CreateVersion7()` for new entity IDs (sequential GUIDs improve indexing).
- **Always store dates as UTC** (`DateTime.UtcNow`).
- Use a `TimeZoneResolver` (or equivalent) for display conversions only.

### Database Operations
- Use **parameterized queries** for any raw SQL (EF Core does this by default with LINQ).
- For read operations, prefer `AsNoTracking()` to avoid unnecessary change-tracking overhead.

---

## 8. Exception Handling

### Golden Rule
- **NEVER throw generic `System.Exception` or `System.ApplicationException`.**
- Always throw a project-specific exception so the global middleware maps it to the correct HTTP status with a descriptive message.

### Exception Layers
Each layer has its own exception namespace, all extending `System.Exception`.

#### Domain Layer (`<Project>.Domain.Exceptions`)
| Exception | HTTP Status | Use Case |
|---|---|---|
| `NotFoundException(entity, key)` | 404 | Entity not found by ID/key |
| `ConflictException(message)` | 409 | Duplicate or conflicting state |
| `InactiveEntityException(entity, key)` | 400 | Operating on inactive/soft-deleted entity |
| `DomainException(message)` | 400 | General business rule violation |

#### Application Layer (`<Project>.Application.Exceptions`)
| Exception | HTTP Status | Use Case |
|---|---|---|
| `ApplicationLayerException(message)` | 400 | Handler-level failures (e.g., `SaveChangesAsync` returned 0) |
| `BadRequestException(message)` | 400 | Invalid input not caught by FluentValidation |

#### Validation
- `FluentValidation.ValidationException` → automatically thrown by the pipeline. Never throw manually.

### Exception Message Format
- **Prefix the message with the handler/command name** for traceability.
- Include the **entity ID or key**.
- Describe **what went wrong and why**.

```csharp
throw new ApplicationLayerException(
    $"{nameof(SampleHandler)}: Failed to save entity with ID '{id}' — SaveChangesAsync returned 0 rows.");
```

### Try-Catch Usage
- Use `try-catch` only for **expected failure scenarios** where you can recover or add context.
- Never use `try-catch` to hide bugs — let unexpected exceptions reach the global middleware.

---

## 9. Logging

- Use **`Microsoft.Extensions.Logging`** for all logging.
- Use **structured logging** with named placeholders (never string interpolation):

```csharp
// ✅ CORRECT — structured
logger.LogInformation("User {UserId} created entity {EntityId}", userId, entityId);

// ❌ INCORRECT — interpolated
logger.LogInformation($"User {userId} created entity {entityId}");
```

- Use **scoped logging** (`logger.BeginScope(...)`) for adding contextual properties to multiple log entries.
- Log levels:
  - `LogTrace` / `LogDebug` — development only
  - `LogInformation` — significant business events
  - `LogWarning` — recoverable anomalies
  - `LogError` — failures that affect a single operation
  - `LogCritical` — failures that affect the system

---

## 10. Resource Management & Localization

- Use **`ResourceManager`** for all user-facing strings (error messages, log messages, UI text).
- Separate resource files by purpose:
  - `LogMessages.resx` — internal log messages
  - `ErrorMessages.resx` — user-facing error messages
- Access resources via:
  ```csharp
  _resourceManager.GetString("MessageKey")
  ```
- Never hardcode user-facing strings inside handlers or services.

---

## 11. Configuration & Secrets

### Configuration Binding
- Use **strongly-typed configuration classes** with data annotations for validation.
- Bind configuration in the `DependencyInjection` class of each layer using `IConfiguration`.
- Configuration source: a single `appsettings.json` file with placeholder/default values.

```csharp
public sealed class DatabaseSettings
{
    [Required]
    public required string ConnectionString { get; init; }

    [Range(1, 300)]
    public int CommandTimeoutSeconds { get; init; } = 30;
}
```

### Secrets & Environment Variables (Single appsettings file)
- **Do not maintain multiple `appsettings.<Environment>.json` files.**
- The `appsettings.json` file contains **only defaults / placeholders**.
- For **local development**, use **.NET User Secrets** (`dotnet user-secrets`) — never commit secrets.
- For **deployed environments**, use **environment variables** to override `appsettings.json` values at runtime. The deployment pipeline injects these values per environment.
- Environment variable naming follows the .NET convention with double underscores (`__`) for nested keys:
  ```text
  Database__ConnectionString
  Logging__LogLevel__Default
  ```
- Sensitive values (connection strings, API keys, tokens) **must never be committed to source control**.

---

## 12. Performance & Security

- Use C# 12+ features and .NET 9 optimizations where applicable.
- Implement **proper input validation and sanitization** at the API boundary.
- Use **parameterized queries** for database operations (EF Core LINQ does this automatically).
- Follow **secure coding practices** — never log sensitive data (passwords, tokens, PII).
- Validate all user input before persisting or acting on it.
- Use HTTPS for all external communication.

---

## 13. Testing

### Project Structure
- One test project per layer:
  - `<Project>.Domain.Tests`
  - `<Project>.Application.Tests`
  - `<Project>.Infrastructure.Tests`
  - `<Project>.Api.Tests`

### Rules
- **Every command must have a test class** in `Application.Tests`.
- Test class naming: `<Handler>Tests` (e.g., `UpsertXCommandHandlerTests`).
- Use **EF Core InMemory Database** for handler tests.
- Use **Moq** for mocking external dependencies and services.
- Use the **builder pattern** for test data setup.

### What to Test
- **Both success and failure scenarios** for every handler.
- **Null parameter validation** — ensure required arguments are validated.
- Edge cases (empty collections, boundary values, invalid states).
- Validators should have their own dedicated test classes.

```csharp
public sealed class UpsertXCommandHandlerTests
{
    private static IProjectDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<ProjectDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ProjectDbContext(options);
    }
}
```

---

## 14. Naming Conventions (C# Standard)

| Element | Convention | Example |
|---|---|---|
| Classes / Records / Structs | PascalCase | `EntityDto` |
| Methods | PascalCase | `GetEntityAsync` |
| Public properties | PascalCase | `FirstName` |
| Private fields | _camelCase | `_dbContext` |
| Local variables | camelCase | `entityId` |
| Parameters | camelCase | `cancellationToken` |
| Constants | PascalCase | `MaxRetries` |
| Interfaces | I + PascalCase | `IProjectDbContext` |

---

## 15. Git Workflow & Branching Strategy

### Protected Branches (no direct commits allowed)
- **`dev`** — Integration branch. All work merges here first.
- **`staging`** — QA / pre-production environment.
- **`master`** — Production environment.

### Working Branch Naming
- `feature/<name>` — New features
- `bugfix/<name>` — Normal bug fixes
- `hotfix/<name>` — Urgent production fixes (branched from `master`)
- `infrastructure/<name>` — Infra/CI/CD changes

### Standard Flow
```text
feature/* → PR → dev → PR → staging → PR → master
```

### Rules
- **Never push directly** to `dev`, `staging`, or `master`.
- **All changes require a Pull Request** with at least 1 approval.
- Use `--no-ff` (no fast-forward) merges to preserve merge history.
- Hotfixes branch from `master`, then **must be synced back** to `staging` and `dev`.

### Commit Messages
- Use clear, descriptive messages in present tense.
- Format: `<type>: <description>` (e.g., `feat: add upsert command for X entity`).

---

## 16. EF Core Migrations

### Environment Order (always in this order)
1. **Local Dev** → Create + test migration first.
2. **QA/Staging** → Apply only after dev is verified.
3. **Production** → Apply manually when team approves release.

### Rules
- **NEVER create migrations against QA or Production databases.**
- Always create migrations using the local development connection.
- Verify the active connection environment variable before running migration commands.
- Migration name must be descriptive (e.g., `Add-Migration AddAuditTrailFields`).# Managermanager Platform — Project Instructions

> **⚠️ Read `GENERAL_INSTRUCTIONS.md` first.** This document only adds Managermanager-specific rules. All general rules (Clean Architecture, CQRS, code style, EF Core conventions, exception handling, testing, Git workflow, etc.) apply here unchanged.

---