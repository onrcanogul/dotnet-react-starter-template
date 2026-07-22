---
name: add-feature
description: Scaffold a complete vertical slice for a new entity in this .NET template - entity, DTO, service contract, Mapperly mapper, service, DbSet with soft-delete filter, DI registration, controller, unit tests and an EF migration. Use when the user asks to add a feature, add an entity, add a CRUD endpoint, create a new resource/model/aggregate, or says something like "add Orders", "I need a Customer entity", or "scaffold X".
---

# Add a feature

Creates every file a new aggregate needs, following the `Products` reference
slice exactly. The value is in matching the existing shape — not in inventing a
new one.

## Before writing anything

1. Read `CLAUDE.md` if it is not already in context.
2. Read the whole `Products` slice. It is the specification:
   - `src/Domain/Template.Domain/Entities/Product.cs`
   - `src/Application/Template.Application.Abstraction/Products/Dtos/ProductDto.cs`
   - `src/Application/Template.Application.Abstraction/Products/IProductService.cs`
   - `src/Application/Template.Application/Products/Mappings/ProductMapper.cs`
   - `src/Application/Template.Application/Products/ProductService.cs`
   - `src/Presentation/Template.WebAPI/Controllers/ProductController.cs`
   - `tests/Template.UnitTests/Services/ProductServiceTests.cs`
3. Confirm the entity's fields with the user if they were not given. Ask once,
   with a concrete proposal, rather than guessing silently:
   > "Adding `Order`. I'll start with `CustomerName: string`, `Total: decimal`,
   > `Status: OrderStatus`. Anything to add or change?"

Naming: entity singular (`Order`), folders plural (`Orders/`), namespaces
follow assembly + folder path with no exceptions.

## Files to create

Create them in this order — each depends on the previous.

**1. Entity** — `src/Domain/Template.Domain/Entities/<Name>.cs`

`public class <Name> : BaseEntity` in `namespace Template.Domain.Entities`.
Reference types get `= null!` or a default; `BaseEntity` already supplies id,
audit columns and `IsDeleted`, so do not redeclare them.

**2. DTO** — `src/Application/Template.Application.Abstraction/<Names>/Dtos/<Name>Dto.cs`

Inherits `BaseDto`. Mirrors the entity's *client-facing* fields only — never
`IsDeleted`, and no internal-only columns.

**3. Service contract** — `src/Application/Template.Application.Abstraction/<Names>/I<Name>Service.cs`

```csharp
public interface I<Name>Service : ICrudService<<Name>, <Name>Dto>
{
    // only what CRUD does not already cover
}
```

If the feature needs nothing beyond CRUD, keep the interface empty — it is
still the DI seam.

**4. Mapper** — `src/Application/Template.Application/<Names>/Mappings/<Name>Mapper.cs`

```csharp
[Mapper]
public partial class <Name>Mapper : IEntityMapper<<Name>, <Name>Dto>
{
    [MapperIgnoreSource(nameof(<Name>.IsDeleted))]
    public partial <Name>Dto ToDto(<Name> entity);

    [MapperIgnoreTarget(nameof(<Name>.IsDeleted))]
    public partial <Name> ToEntity(<Name>Dto dto);

    public partial List<<Name>Dto> ToDtoList(IEnumerable<<Name>> entities);

    [MapperIgnoreTarget(nameof(<Name>.Id))]
    [MapperIgnoreTarget(nameof(<Name>.IsDeleted))]
    [MapperIgnoreTarget(nameof(<Name>.CreatedDate))]
    [MapperIgnoreTarget(nameof(<Name>.CreatedBy))]
    [MapperIgnoreTarget(nameof(<Name>.UpdatedDate))]
    [MapperIgnoreTarget(nameof(<Name>.UpdatedBy))]
    [MapperIgnoreSource(nameof(<Name>Dto.Id))]
    [MapperIgnoreSource(nameof(<Name>Dto.CreatedDate))]
    [MapperIgnoreSource(nameof(<Name>Dto.CreatedBy))]
    [MapperIgnoreSource(nameof(<Name>Dto.UpdatedDate))]
    [MapperIgnoreSource(nameof(<Name>Dto.UpdatedBy))]
    public partial void Apply(<Name>Dto dto, <Name> target);
}
```

The `Apply` ignores are mandatory: identity and audit columns belong to the
persistence layer, and without them a request body can overwrite `CreatedBy`.

**5. Service** — `src/Application/Template.Application/<Names>/<Name>Service.cs`

```csharp
public class <Name>Service(
        IRepository<<Name>> repository,
        IEntityMapper<<Name>, <Name>Dto> mapper,
        IUnitOfWork unitOfWork,
        IStringLocalizer localize)
    : CrudService<<Name>, <Name>Dto>(repository, mapper, unitOfWork, localize), I<Name>Service
{
}
```

Inside the body use the inherited `Repository` / `Mapper` / `UnitOfWork` /
`Localize`. Referencing the constructor parameters instead stores each
dependency twice and trips CS9107. Add `ILogger<<Name>Service>` only if you
actually log.

**6. DbSet + query filter** — `src/Infrastructure/Template.Persistence/Contexts/TemplateDbContext.cs`

```csharp
public DbSet<<Name>> <Names> { get; set; }
```

and in `OnModelCreating`:

```csharp
modelBuilder.Entity<<Name>>().HasQueryFilter(x => !x.IsDeleted);
```

Skipping the filter means soft-deleted rows come back in every query.

**7. DI** — `src/Application/Template.Application/ServiceRegistration.cs`

```csharp
services.AddSingleton<IEntityMapper<<Name>, <Name>Dto>, <Name>Mapper>();
services.AddScoped<I<Name>Service, <Name>Service>();
```

Mappers are stateless, hence singleton; services are scoped.

**8. Controller** — `src/Presentation/Template.WebAPI/Controllers/<Name>Controller.cs`

Derive from `BaseController` (it supplies `[ApiController]` and the route — do
not repeat them). Every action is one line handing a `ServiceResponse` to
`ApiResult`. Reads are `GET`, including search. XML doc comments feed Swagger.

**9. Tests** — `tests/Template.UnitTests/Services/<Name>ServiceTests.cs` and
`tests/Template.UnitTests/Controllers/<Name>ControllerTests.cs`

Mock `IRepository<T>`, `IUnitOfWork` and `IStringLocalizer`; use the **real**
mapper — it is a generated pure function, and mocking it would only assert that
the test agrees with itself. Cover at minimum: create, read-found,
read-not-found (throws `NotFoundException`), update, delete, and that `Apply`
leaves audit columns untouched.

**10. Migration**

```bash
export DOTNET_ROOT="$(dirname "$(readlink -f "$(which dotnet)")")"
dotnet ef migrations add Add<Name> \
  --project src/Infrastructure/Template.Persistence \
  --startup-project src/Presentation/Template.WebAPI \
  --context TemplateDbContext
```

`--context` is required; the solution has more than one `DbContext`.

## Verify before reporting done

```bash
dotnet build Template.sln           # 0 errors AND 0 warnings
dotnet test Template.sln
python3 scripts/check-namespaces.py
```

A Mapperly diagnostic (`RMG…`) means a property genuinely cannot be mapped.
Fix the mapping or add a deliberate ignore with a reason — never suppress it to
make the build quiet.

State what you ran and what it returned. If a step failed and you did not fix
it, say which.
