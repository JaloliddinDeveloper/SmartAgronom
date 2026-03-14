# ── Stage 1: Build ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files for layer caching
COPY ["src/AqlliAgronom.Domain/AqlliAgronom.Domain.csproj", "AqlliAgronom.Domain/"]
COPY ["src/AqlliAgronom.Application/AqlliAgronom.Application.csproj", "AqlliAgronom.Application/"]
COPY ["src/AqlliAgronom.Infrastructure/AqlliAgronom.Infrastructure.csproj", "AqlliAgronom.Infrastructure/"]
COPY ["src/AqlliAgronom.API/AqlliAgronom.API.csproj", "AqlliAgronom.API/"]

# Restore dependencies
RUN dotnet restore "AqlliAgronom.API/AqlliAgronom.API.csproj"

# Copy all source
COPY src/ .

# Build
RUN dotnet build "AqlliAgronom.API/AqlliAgronom.API.csproj" -c Release -o /app/build --no-restore

# ── Stage 2: Publish ──────────────────────────────────────────────────────────
FROM build AS publish
RUN dotnet publish "AqlliAgronom.API/AqlliAgronom.API.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# ── Stage 3: Runtime ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Security: run as non-root
RUN groupadd -r appgroup && useradd -r -g appgroup appuser

# Install curl for health checks
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/logs && chown -R appuser:appgroup /app

USER appuser

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:8080/health/live || exit 1

ENTRYPOINT ["dotnet", "AqlliAgronom.API.dll"]
