using AqlliAgronom.API.Middleware;
using AqlliAgronom.Application;
using AqlliAgronom.Application.Common.Interfaces;
using AqlliAgronom.Domain.Entities;
using AqlliAgronom.Domain.Enums;
using AqlliAgronom.Infrastructure;
using AqlliAgronom.Infrastructure.Persistence;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

// ── Bootstrap logger (before DI container is built) ──────────────────────────
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting AqlliAgronom API...");

    var builder = WebApplication.CreateBuilder(args);

    // ── Serilog full configuration from appsettings.json ─────────────────────
    builder.Host.UseSerilog((context, services, configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName());

    // ── Application & Infrastructure layers ──────────────────────────────────
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    // ── ASP.NET Core services ─────────────────────────────────────────────────
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    // ── Swagger/OpenAPI ───────────────────────────────────────────────────────
    builder.Services.AddSwaggerGen(opts =>
    {
        opts.SwaggerDoc("v1", new() { Title = "AqlliAgronom AI API", Version = "v1" });

        // JWT Bearer auth in Swagger UI
        opts.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Enter: Bearer {your-token}"
        });
        opts.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                []
            }
        });
    });

    // ── CORS ──────────────────────────────────────────────────────────────────
    builder.Services.AddCors(opts =>
    {
        opts.AddPolicy("AllowAll", policy =>
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

        opts.AddPolicy("Production", policy =>
            policy
                .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [])
                .AllowAnyMethod()
                .AllowAnyHeader());
    });

    // ── Rate Limiting ─────────────────────────────────────────────────────────
    builder.Services.AddRateLimiter(opts =>
    {
        opts.AddSlidingWindowLimiter("global", limiter =>
        {
            limiter.PermitLimit = 200;
            limiter.Window = TimeSpan.FromMinutes(1);
            limiter.SegmentsPerWindow = 4;
        });

        opts.AddSlidingWindowLimiter("ai_chat", limiter =>
        {
            limiter.PermitLimit = 20;
            limiter.Window = TimeSpan.FromMinutes(1);
            limiter.SegmentsPerWindow = 4;
        });

        opts.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    });

    // ── Health Checks ─────────────────────────────────────────────────────────
    builder.Services.AddHealthChecks()
        .AddNpgSql(
            connectionString: builder.Configuration.GetConnectionString("PostgreSQL")!,
            name: "postgresql",
            tags: ["db", "infra"])
        .AddRedis(
            builder.Configuration.GetSection("Redis:ConnectionString").Value ?? "localhost:6379",
            name: "redis",
            tags: ["cache", "infra"]);

    var app = builder.Build();

    // ── Auto-migrate database on startup ──────────────────────────────────────
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();
        Log.Information("Database migrations applied.");

        // ── Seed default admin user if none exists ─────────────────────────
        var adminPhone = "+998900000001";
        var adminExists = await db.Users.AnyAsync(u => u.Role == UserRole.Admin);
        if (!adminExists)
        {
            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            var adminPassword = builder.Configuration["Seed:AdminPassword"] ?? "Admin@12345";
            var admin = User.Register(
                fullName: "Admin",
                phone: adminPhone,
                passwordHash: hasher.Hash(adminPassword));
            admin.AssignRole(UserRole.Admin);
            db.Users.Add(admin);
            await db.SaveChangesAsync();
            Log.Information("Default admin user seeded (phone: {Phone})", adminPhone);
        }

        // ── Link AdminTelegramChatId to admin user ─────────────────────────
        var adminChatIdStr = builder.Configuration["Seed:AdminTelegramChatId"];
        if (long.TryParse(adminChatIdStr, out var adminChatId) && adminChatId > 0)
        {
            // Find admin user
            var adminUser = await db.Users.FirstOrDefaultAsync(u => u.Role == UserRole.Admin);
            if (adminUser is not null && adminUser.TelegramChatId != adminChatId)
            {
                adminUser.LinkTelegram(adminChatId, null);
                await db.SaveChangesAsync();
                Log.Information("Admin TelegramChatId linked: {ChatId}", adminChatId);
            }

            // Also: if a user with this ChatId exists but is not Admin — promote them
            var userWithChatId = await db.Users.FirstOrDefaultAsync(u => u.TelegramChatId == adminChatId);
            if (userWithChatId is not null && userWithChatId.Role != UserRole.Admin)
            {
                userWithChatId.AssignRole(UserRole.Admin);
                await db.SaveChangesAsync();
                Log.Information("User {UserId} promoted to Admin via AdminTelegramChatId", userWithChatId.Id);
            }
        }
    }

    // ── Middleware pipeline (ORDER MATTERS) ───────────────────────────────────
    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseMiddleware<RequestLoggingMiddleware>();

    app.UseSerilogRequestLogging(opts =>
    {
        opts.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.000}ms";
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(opts =>
        {
            opts.SwaggerEndpoint("/swagger/v1/swagger.json", "AqlliAgronom AI v1");
            opts.RoutePrefix = string.Empty; // Serve at root
        });
        app.UseCors("AllowAll");
    }
    else
    {
        app.UseHsts();
        app.UseCors("Production");
    }

    app.UseHttpsRedirection();
    app.UseRateLimiter();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    // Health check endpoints
    app.MapHealthChecks("/health");
    app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        Predicate = _ => false // Liveness only — no actual checks
    });

    Log.Information("AqlliAgronom API started on {Urls}", string.Join(", ", app.Urls));
    await app.RunAsync();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "AqlliAgronom API terminated unexpectedly.");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

return 0;
