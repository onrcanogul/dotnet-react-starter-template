# Build from the repository root so the other projects are in context:
#   docker build -f src/Presentation/Template.WebAPI/Dockerfile -t template-api .
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Restore first, on the project graph alone, so a source-only change does not
# invalidate the package cache.
COPY Directory.Build.props Directory.Packages.props global.json Template.sln ./
COPY src/Shared/Template.Shared/*.csproj                              src/Shared/Template.Shared/
COPY src/Domain/Template.Domain/*.csproj                              src/Domain/Template.Domain/
COPY src/Application/Template.Application.Abstraction/*.csproj        src/Application/Template.Application.Abstraction/
COPY src/Application/Template.Application/*.csproj                    src/Application/Template.Application/
COPY src/Infrastructure/Template.Persistence/*.csproj                 src/Infrastructure/Template.Persistence/
COPY src/Infrastructure/Template.Infrastructure/*.csproj              src/Infrastructure/Template.Infrastructure/
COPY src/Presentation/Template.WebAPI/*.csproj                        src/Presentation/Template.WebAPI/
RUN dotnet restore src/Presentation/Template.WebAPI/Template.WebAPI.csproj

COPY src/ src/
RUN dotnet publish src/Presentation/Template.WebAPI/Template.WebAPI.csproj \
        -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app

# Non-root: nothing here needs to write to the filesystem.
RUN adduser --disabled-password --no-create-home --uid 10001 appuser
USER appuser

COPY --from=build --chown=appuser /app .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production

# Configuration comes from the environment - the image carries no secrets.
ENTRYPOINT ["dotnet", "Template.WebAPI.dll"]
