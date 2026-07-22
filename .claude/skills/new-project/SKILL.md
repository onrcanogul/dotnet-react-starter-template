---
name: new-project
description: Turn this template into a fresh project by renaming every Template.* identifier, project, folder, database and container to a new name, then optionally removing the sample Product feature. Use when the user says they are starting a new project from the template, wants to rename Template to something else, or asks to bootstrap/scaffold/initialise a new service from this base.
---

# Start a new project from the template

Renames the template into a real project. Mechanical, but wide — it touches
project files, namespaces, folder names, the solution, Docker resources and
the migration history, so the ordering below matters.

## Inputs

Ask for these once, together, if not supplied:

1. **Project name** in PascalCase — `OrderApi`, `Billing`, `Crm`. This replaces
   `Template` everywhere. Reject a name that is not a valid C# identifier.
2. **Keep the sample `Product` feature?** Default no. It exists to demonstrate
   the slice; once the first real feature lands it is noise.

Confirm the plan before touching anything:

> "Renaming `Template` → `OrderApi` across 8 projects, the solution, namespaces,
> Docker containers and the database name, and removing the sample Product
> feature. This rewrites most files in the repo. Proceed?"

## Preconditions

- Working tree is clean (`git status --porcelain` empty). If not, stop and ask
  — this rewrites too much to untangle from unrelated edits.
- Build and tests are green *before* starting, so any breakage afterwards is
  clearly attributable.

## Steps

**1. Rename directories and files.** Bottom-up, so parent renames do not
invalidate child paths:

- `src/*/Template.*` → `src/*/<Name>.*` (8 project directories)
- `Template.*.csproj` → `<Name>.*.csproj`
- `Template.sln` → `<Name>.sln`
- `src/Presentation/Clients/template-web-ui` → `<name-kebab>-web-ui`
- `TemplateDbContext.cs` → `<Name>DbContext.cs`

Use `git mv` to keep history. On macOS a case-only rename needs two steps via
a temporary name.

**2. Rewrite identifiers.** Across `*.cs`, `*.csproj`, `*.sln`, `*.json`,
`*.props`, `*.yml`, `*.md`:

- `Template.` → `<Name>.` (namespaces, project references)
- `TemplateDbContext` → `<Name>DbContext`
- `Template-DB` → `<Name>-DB`
- `template-app` → `<name-kebab>-app` (Redis instance prefix)
- `template-` container names → `<name-kebab>-`
- `template-web-ui` → `<name-kebab>-web-ui` (also `package.json` `name`)
- bare `Template` in prose and in `OpenTelemetry.ServiceName`

Order longest-first so a short match never eats a longer one. Do not touch
`obj/`, `bin/`, `node_modules/`, `dist/`.

**3. Reset project identity.**

- new `<UserSecretsId>` GUID in the API csproj
- `.template.config/`, if present, is removed
- `README.md` — retitle, drop template-specific sections
- `CLAUDE.md` — keep it; the conventions are the point. Update the name and
  any command paths.

**4. Remove the sample feature** (unless keeping it). Delete:

- `Domain/Entities/Product.cs`
- `Application.Abstraction/Products/`
- `Application/Products/`
- `WebAPI/Controllers/ProductController.cs`
- `tests/**/Product*Tests.cs`
- the `Products` `DbSet` and its query filter in the DbContext
- the Product registrations in `ServiceRegistration`

Keep `Base/`, `Users/`, `Shared/` and every cross-cutting extension — that is
the actual template.

If the feature is removed, `CLAUDE.md` and `.claude/skills/add-feature` both
point at `Products` as the reference. Update those references to name the
first real feature instead, or note that the reference slice is gone.

**5. Reset migrations.** The template's migration describes the template's
schema.

```bash
rm -rf src/Infrastructure/<Name>.Persistence/Migrations
export DOTNET_ROOT="$(dirname "$(readlink -f "$(which dotnet)")")"
dotnet ef migrations add InitialCreate \
  --project src/Infrastructure/<Name>.Persistence \
  --startup-project src/Presentation/<Name>.WebAPI \
  --context <Name>DbContext
```

**6. Reset git history** — only if the user asks. Default is to keep it and
add a commit; a fresh `git init` throws away the template's history, which is
sometimes wanted and sometimes a mistake. Ask rather than assume.

## Verify

```bash
dotnet build <Name>.sln              # 0 errors, 0 warnings
dotnet test <Name>.sln
python3 scripts/check-namespaces.py
cd src/Presentation/<name-kebab>-web-ui && npm run build
grep -ri "template" --include="*.cs" --include="*.csproj" --include="*.json" src/ tests/
```

The final `grep` should return nothing outside `node_modules`/`obj`/`bin`. Any
hit is a rename that was missed — chase it down rather than reporting success.

Then tell the user what to do next: set the JWT secret via user-secrets, start
compose, run `database update`.
