# ── Build Arguments ───────────────────────────────────────────────────────────
ARG BUILD_VERSION=0.0.0
ARG BUILD_REVISION=local

# ── Stage 1: Restore ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore
WORKDIR /src

# Copy project files first — this layer is cached until any .csproj changes
COPY ["src/AqlliAgronom.Domain/AqlliAgronom.Domain.csproj",              "AqlliAgronom.Domain/"]
COPY ["src/AqlliAgronom.Application/AqlliAgronom.Application.csproj",   "AqlliAgronom.Application/"]
COPY ["src/AqlliAgronom.Infrastructure/AqlliAgronom.Infrastructure.csproj", "AqlliAgronom.Infrastructure/"]
COPY ["src/AqlliAgronom.API/AqlliAgronom.API.csproj",                   "AqlliAgronom.API/"]

RUN dotnet restore "AqlliAgronom.API/AqlliAgronom.API.csproj"

# ── Stage 2: Publish ──────────────────────────────────────────────────────────
FROM restore AS publish

# Copy all source code
COPY src/ .

# Build + publish in one step (avoids the --no-build path resolution issue)
RUN dotnet publish "AqlliAgronom.API/AqlliAgronom.API.csproj" \
    --configuration Release \
    --no-restore \
    -o /app/publish \
    /p:UseAppHost=false

# ── Stage 3: Runtime ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

ARG BUILD_VERSION
ARG BUILD_REVISION

LABEL org.opencontainers.image.title="AqlliAgronom API" \
      org.opencontainers.image.description="AI-powered smart agriculture assistant backend" \
      org.opencontainers.image.version="${BUILD_VERSION}" \
      org.opencontainers.image.revision="${BUILD_REVISION}" \
      org.opencontainers.image.vendor="AqlliAgronom"

WORKDIR /app

# Install curl for health checks (in a single layer to minimize image size)
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

# Security: run as non-root user
RUN groupadd -r appgroup && useradd -r -g appgroup -s /sbin/nologin appuser

COPY --from=publish /app/publish .

# Create logs directory with correct ownership
RUN mkdir -p /app/logs && chown -R appuser:appgroup /app

USER appuser

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

HEALTHCHECK --interval=30s --timeout=10s --start-period=90s --retries=3 \
    CMD curl -sf http://localhost:8080/health/live || exit 1

ENTRYPOINT ["dotnet", "AqlliAgronom.API.dll"]
