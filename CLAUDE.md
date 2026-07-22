# Template — .NET 8 Onion API + React SPA

Starter template that new projects are branched from. Treat every convention
here as load-bearing: whatever shape you leave the code in is the shape the
next feature — and the next project — will copy.

## Commands

```bash
docker compose up -d                     # Postgres, Redis, Elasticsearch, Jaeger, OTLP collector
dotnet build Template.sln                # must stay at 0 warnings
dotnet test tests/Template.UnitTests     # fast, no dependencies
dotnet test tests/Template.IntegrationTests   # boots the real app; needs Docker
dotnet run --project src/Presentation/Template.WebAPI   # http://localhost:5000
```

`dotnet test Template.sln` runs both suites. The integration suite starts a
throwaway Postgres container via Testcontainers, so Docker must be running —
it is what catches startup wiring, middleware order and migration failures
that unit tests structurally cannot see.

Frontend lives in `src/Presentation/Clients/template-web-ui`:

```bash
npm install && npm run dev               # http://localhost:3000
npm run build                            # tsc -b && vite build; typecheck runs here, not in dev
npm run lint
```

Migrations (`dotnet-ef` needs `DOTNET_ROOT` set on macOS/Homebrew):

```bash
export DOTNET_ROOT="$(dirname "$(readlink -f "$(which dotnet)")")"
dotnet ef migrations add <Name> --project src/Infrastructure/Template.Persistence --startup-project src/Presentation/Template.WebAPI --context TemplateDbContext
dotnet ef database update --project src/Infrastructure/Template.Persistence --startup-project src/Presentation/Template.WebAPI --context TemplateDbContext
```

Secrets are never committed. `appsettings.json` ships with empty values; supply
real ones locally:

```bash
cd src/Presentation/Template.WebAPI
dotnet user-secrets set "Token:SecurityKey" "$(openssl rand -base64 48)"
```

## Layer rules

Dependencies point inward only. `Template.Application` must never reference
`Template.Infrastructure` or `Template.Persistence` — put the interface in
`Template.Application.Abstraction` and let the outer layer implement it.

```
Shared  ←  Domain  ←  Application.Abstraction  ←  Application
                                    ↑                  ↑
                          Persistence / Infrastructure ┘
                                    ↑
                                 WebAPI          (composition root: DI is wired here)
```

`Template.Shared` sits at the bottom and depends on nothing in the solution.
`Template.WebAPI` is the only project allowed to know about all the others.

## Namespace and folder rule

**`namespace` == assembly name + folder path.** No exceptions — it is checked
in CI by `scripts/check-namespaces.py`.

- Folders are `PascalCase`.
- Feature folders are **plural** (`Products/`, `Users/`), types inside are
  singular (`Product`, `ProductDto`). A singular folder collides with its own
  type name and forces awkward qualification.
- Interfaces live next to their implementation, except Application contracts,
  which live in `Template.Application.Abstraction` mirroring the same paths.
- Never introduce a `src/` segment inside a project.

## The vertical slice

Everything a feature needs, in the order to create it. `Products` is the
reference implementation — read it before writing a new one.

| # | File | Purpose |
|---|------|---------|
| 1 | `Domain/Template.Domain/Entities/<Name>.cs` | entity, inherits `BaseEntity` |
| 2 | `Application.Abstraction/<Names>/Dtos/<Name>Dto.cs` | DTO, inherits `BaseDto` |
| 3 | `Application.Abstraction/<Names>/I<Name>Service.cs` | extends `ICrudService<TEntity, TDto>`, declares only the extras |
| 4 | `Application/<Names>/Mappings/<Name>Mapper.cs` | `[Mapper] partial class`, implements `IEntityMapper<TEntity, TDto>` |
| 5 | `Application/<Names>/<Name>Service.cs` | inherits `CrudService<TEntity, TDto>`, implements the interface |
| 6 | `Persistence/Contexts/TemplateDbContext.cs` | add the `DbSet<>` and a soft-delete query filter |
| 7 | `Application/ServiceRegistration.cs` | register mapper + service |
| 8 | `WebAPI/Controllers/<Name>Controller.cs` | derives `BaseController`, one-line actions |
| 9 | `tests/Template.UnitTests/...` | service and controller tests |
| 10 | migration | see the command above |

`/add-feature` scaffolds all ten. Prefer it over doing this by hand.

## Conventions that matter

**Responses.** Services return `ServiceResponse<T>`; controllers return
`ApiResult(...)` and contain no logic. Failures are thrown as the exception
types in `Template.Shared.Exceptions` — `ExceptionHandler` maps them to status
codes. Any other exception becomes a logged 500 with a generic message, so
never surface internal detail by throwing a bare `Exception`.

**Mapping is compile-time.** Mapperly generates the bodies; a property it
cannot map is a build error. That is the point — do not silence a diagnostic
without understanding it. `Apply()` must ignore `Id` and the audit columns:
those belong to the persistence layer, and letting a DTO write them is how a
client overwrites `CreatedBy`.

Hand-write a mapper only where a generated one would be mostly suppressions —
`UserMapper` is the one such case, because `User` inherits a dozen Identity
columns.

**Inheriting `CrudService`.** Use the protected `Repository` / `Mapper` /
`UnitOfWork` / `Localize` members. Re-declaring the same dependency in the
derived primary constructor stores it twice and trips CS9107.

**Persistence.** Repositories never commit — `IUnitOfWork.CommitAsync()` is
called once per use case. Deletes are soft (`IsDeleted`), and reads are
filtered by a global query filter, so a new entity needs its own
`HasQueryFilter`. Reads default to no-tracking; pass `disableTracking: false`
when you intend to mutate.

**Audit columns.** Written only by `TemplateDbContext.AuditingEntities`.
Nothing else assigns `CreatedBy` / `CreatedDate` / `UpdatedBy` / `UpdatedDate`.

**Config.** Read required settings through `GetRequired` /
`GetRequiredConnectionString` so a missing key fails at startup naming itself,
rather than as a null reference mid-request. Never use `!` on a config lookup.

**Packages.** Versions live only in `Directory.Packages.props`; `<PackageReference>`
carries no `Version`. Framework, nullability and language version live only in
`Directory.Build.props`.

**Localisation.** Every `Localize["Key"]` needs the key present in *both*
`WebAPI/Resources/localization.en-US.json` and `localization.tr-TR.json`; a
missing key renders as the raw key name to the user.

## Frontend conventions

Token storage goes through `src/api/tokenStorage.ts` — never touch
`localStorage` for tokens directly. API errors go through
`apiErrorMessage(error, fallback)`; reaching into `error.response.data` throws
on network failures, which have no response.

Auth state lives in `features/authSlice.ts`. Identity is decoded from the
access token, not read from a response body. Every user-facing string is an
i18n key present in both `public/locales/{en,tr}/translation.json`.

## Definition of done

Before calling any change complete:

1. `dotnet build Template.sln` — **0 errors, 0 warnings**. The build is
   warning-free today; a new warning is a regression, not background noise.
2. `dotnet test Template.sln` — green, with tests covering the change.
3. `dotnet format Template.sln --verify-no-changes` — clean (CI fails on this).
4. `python3 scripts/check-namespaces.py` — clean.
5. Frontend touched? `npm run build` in the client directory (this is what
   typechecks; `npm run dev` does not).
6. New config key? Added to `appsettings.json` (empty) and documented.
7. New entity? Migration generated and committed.
8. New endpoint? An integration test covering it, not only a mocked unit test.

Report what you actually ran. If something fails and you did not fix it, say so.

## Gotchas

- `dotnet ef` fails with "You must install .NET" unless `DOTNET_ROOT` is set.
- The solution has two `DbContext` types (ours plus health-checks UI), so
  `dotnet ef` always needs `--context TemplateDbContext`.
- macOS is case-insensitive: renaming a folder's casing needs two `git mv`
  steps. CI runs on Linux, where a wrong-case path is a hard failure — this
  already bit `Resources/`.
- `Token` and `TokenHandler` collide with `Microsoft.IdentityModel.Tokens`;
  ours are `Tokens` and `JwtTokenHandler`.
- The API runs on the host, not in compose — compose only provides the
  backing services.
