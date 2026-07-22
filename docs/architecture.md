# Architecture

`CLAUDE.md` is the operating contract — what the rules *are*. This document is
the reasoning behind them, for when you need to decide whether a rule still
applies to a case it did not anticipate.

## Why Onion

The dependency arrows all point inward, so the domain and the use cases can be
compiled, read and tested without a database, an HTTP server, or a search
cluster in the picture.

```
┌──────────────────────────────────────────────────────────────┐
│ Template.WebAPI                        composition root      │
│ controllers · DI wiring · middleware · Swagger · telemetry   │
└───────────────┬──────────────────────────────┬───────────────┘
                │                              │
┌───────────────▼──────────────┐  ┌────────────▼───────────────┐
│ Template.Infrastructure      │  │ Template.Persistence       │
│ JWT · exception handling     │  │ EF Core · repos · UoW      │
└───────────────┬──────────────┘  └────────────┬───────────────┘
                │                              │
┌───────────────▼──────────────────────────────▼───────────────┐
│ Template.Application            use cases, one per feature   │
└───────────────────────────┬──────────────────────────────────┘
                            │
┌───────────────────────────▼──────────────────────────────────┐
│ Template.Application.Abstraction     contracts only          │
└───────────────────────────┬──────────────────────────────────┘
                            │
┌───────────────────────────▼──────────────────────────────────┐
│ Template.Domain              entities                        │
└───────────────────────────┬──────────────────────────────────┘
                            │
┌───────────────────────────▼──────────────────────────────────┐
│ Template.Shared     BaseEntity · BaseDto · ServiceResponse   │
│                     exceptions · config guards               │
└──────────────────────────────────────────────────────────────┘
```

The rule that earns its keep: **`Template.Application` may not reference
`Template.Infrastructure` or `Template.Persistence`.** When a use case needs
something from the outside — a repository, a token issuer, a search client —
the interface goes in `Template.Application.Abstraction` and the outer project
implements it. Break this once and the layering becomes decorative: the
application can no longer be reasoned about, or tested, without dragging a
database along.

## Request path

```
HTTP  →  Controller  →  Service          →  Repository  →  DbContext
                          ↓ Mapper
                     ServiceResponse<T>
                          ↓
                  BaseController.ApiResult  →  HTTP status + body
```

Controllers hold no logic. Every action is one line: call the service, hand the
result to `ApiResult`. This keeps the HTTP layer swappable and means the
interesting behaviour is always in a class that unit tests can construct
directly.

Errors travel as exceptions rather than as failed `ServiceResponse` values, so
a use case does not have to thread a failure through every intermediate return.
`ExceptionHandler` maps the types in `Template.Shared.Exceptions` to status
codes; anything else is a bug and becomes a logged 500 with a generic body, so
internal detail cannot leak to a caller.

## Why compile-time mapping

Reflection-based mapping fails at runtime, on a request, in production. A
generated mapper fails at `dotnet build`.

That matters more than usual here, because this template is written and
extended largely by agents whose verification loop *is* the build. A mapping
mistake that only shows up under a specific request is invisible to that loop;
one that breaks the build is caught immediately. Adopting Mapperly surfaced two
existing defects on the first compile — a soft-delete flag leaking into a DTO,
and a registration field silently discarded.

The cost is that mappings must be declared rather than inferred. That is the
trade being made deliberately.

## Data model conventions

Everything inheriting `BaseEntity` gets an identity, four audit columns and a
soft-delete flag.

- **Audit columns** are written in exactly one place,
  `TemplateDbContext.AuditingEntities`, from the signed-in user or `"system"`.
  Nothing else assigns them, and mappers explicitly refuse to — otherwise a
  client could overwrite `CreatedBy` by putting it in a request body.
- **Soft delete** means `Delete` sets `IsDeleted` and a global query filter
  hides the row. A new entity needs its own `HasQueryFilter`, or deleted rows
  reappear in its queries.
- **Tracking** is off by default for reads, and turned on explicitly for the
  read-modify-write in `UpdateAsync`/`DeleteAsync`.

## Where the seams are

Deliberate extension points, each already used once so the pattern is visible:

| Seam | Contract | Reference implementation |
|------|----------|--------------------------|
| Persistence | `IRepository<T>`, `IUnitOfWork` | EF Core over Postgres |
| Mapping | `IEntityMapper<TEntity, TDto>` | `ProductMapper` (generated) |
| Search | `IElasticSearchService` | Elasticsearch |
| Cache | `ICacheService` | Redis and in-memory |
| Auth | `IJwtTokenHandler` | JWT bearer |
| Config | `GetRequired` | fail-fast at startup |

## Observability

The API exports OTLP to the collector when `OTEL_EXPORTER_OTLP_ENDPOINT` is
set, and to the console otherwise — so a bare `dotnet run` still shows traces
without any infrastructure. The collector forwards to Jaeger
(http://localhost:16686). Serilog handles structured logs; health checks are at
`/health`, with a UI at `/health-ui`.

## Deliberate omissions

Not present, and that is a choice rather than an oversight:

- **CQRS / MediatR.** The service layer is the use-case boundary. Add a
  mediator when features genuinely need pipeline behaviours, not by default.
- **Cancellation tokens.** Not threaded through the service and repository
  contracts yet. Worth doing before the first long-running endpoint.
- **Validation library.** Validation currently lives in services and throws
  `BadRequestException`. FluentValidation would fit at the controller boundary
  if input validation grows.
- **Domain events.** No dispatcher. Add one when a write needs to trigger work
  beyond its own aggregate.
