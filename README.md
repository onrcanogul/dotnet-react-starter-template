# .NET 8 + React Starter Template

Production-shaped starting point for a web application: an Onion-architecture
.NET 8 API and a React 18 SPA, with the cross-cutting concerns already wired —
auth, caching, search, telemetry, health checks, localisation.

It is also built to be worked on by coding agents. `CLAUDE.md` states the
conventions as an enforceable contract, `.claude/skills/` scaffolds whole
vertical slices, and CI checks the rules a reviewer would otherwise have to
remember.

## Features

| | |
|---|---|
| **Onion architecture** | dependencies point inward; enforced in CI |
| **Auth** | ASP.NET Identity + JWT with refresh tokens |
| **Persistence** | EF Core / PostgreSQL, repository + unit of work, soft delete, audit columns |
| **Mapping** | Mapperly — source-generated, so mapping errors are build errors |
| **Search** | Elasticsearch |
| **Caching** | Redis and in-memory, behind one interface |
| **Observability** | OpenTelemetry → OTLP collector → Jaeger, Serilog structured logs |
| **Ops** | health checks + UI, rate limiting, global exception handling |
| **i18n** | JSON resources on both API and client |
| **Frontend** | React 18, TypeScript, Vite, Redux Toolkit, i18next |

## Getting started

Requires the [.NET 8 SDK](https://dotnet.microsoft.com/download), Node 18+ and
Docker.

```bash
docker compose up -d          # Postgres, Redis, Elasticsearch, Jaeger, OTLP collector
```

Set the JWT signing key (secrets are never committed — `appsettings.json` ships
with empty values):

```bash
cd src/Presentation/Template.WebAPI
dotnet user-secrets set "Token:SecurityKey" "$(openssl rand -base64 48)"
```

Create the schema, then run the API:

```bash
export DOTNET_ROOT="$(dirname "$(readlink -f "$(which dotnet)")")"
dotnet ef database update --project src/Infrastructure/Template.Persistence --startup-project src/Presentation/Template.WebAPI --context TemplateDbContext
dotnet run --project src/Presentation/Template.WebAPI
```

And the client:

```bash
cd src/Presentation/Clients/template-web-ui
npm install && npm run dev
```

| | |
|---|---|
| API | http://localhost:5000 |
| Swagger | http://localhost:5000/swagger |
| Health UI | http://localhost:5000/health-ui |
| Web UI | http://localhost:3000 |
| Jaeger | http://localhost:16686 |
| Kibana | http://localhost:5601 |

## Layout

```
src/
  Shared/Template.Shared                         base types, envelopes, exceptions
  Domain/Template.Domain                         entities
  Application/Template.Application.Abstraction   contracts
  Application/Template.Application               use cases
  Infrastructure/Template.Persistence            EF Core, repositories, unit of work
  Infrastructure/Template.Infrastructure         JWT, middleware
  Presentation/Template.WebAPI                   controllers, DI, cross-cutting setup
  Presentation/Clients/template-web-ui           React SPA
tests/Template.UnitTests
```

`Products` is the reference feature — copy its shape when adding one.

## Starting a project from this template

With Claude Code:

```
/new-project OrderApi
```

That renames every `Template.*` identifier, project, folder and container,
removes the sample `Product` feature, and regenerates the initial migration.

## Working on it

Read [`CLAUDE.md`](CLAUDE.md) first — it is short, and it is what keeps the
codebase consistent enough to extend safely.
[`docs/architecture.md`](docs/architecture.md) explains the reasoning behind
those rules.

```bash
dotnet build Template.sln           # must stay at 0 warnings
dotnet test Template.sln
python3 scripts/check-namespaces.py
```
