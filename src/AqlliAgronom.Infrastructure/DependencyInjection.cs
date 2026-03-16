//-------- 
using AqlliAgronom.Application.AI.Interfaces;
using AqlliAgronom.Application.Common.Interfaces;
using AqlliAgronom.Domain.Interfaces;
using AqlliAgronom.Domain.Interfaces.Repositories;
using AqlliAgronom.Infrastructure.AI;
using AqlliAgronom.Infrastructure.AI.Claude;
using AqlliAgronom.Infrastructure.AI.Embedding;
using AqlliAgronom.Infrastructure.AI.Qdrant;
using AqlliAgronom.Infrastructure.BackgroundJobs;
using AqlliAgronom.Infrastructure.Caching;
using AqlliAgronom.Infrastructure.Identity;
using AqlliAgronom.Infrastructure.Persistence;
using AqlliAgronom.Infrastructure.Persistence.Interceptors;
using AqlliAgronom.Infrastructure.Persistence.Repositories;
using AqlliAgronom.Infrastructure.Services;
using AqlliAgronom.Infrastructure.Telegram;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Qdrant.Client;
using StackExchange.Redis;
using System.Text;
using Telegram.Bot;

namespace AqlliAgronom.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Database ─────────────────────────────────────────────────────────
        services.AddDbContext<ApplicationDbContext>(opts =>
            opts.UseNpgsql(
                    configuration.GetConnectionString("PostgreSQL"),
                    npgsql =>
                    {
                        npgsql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(10), null);
                        npgsql.CommandTimeout(30);
                        npgsql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    })
                .UseSnakeCaseNamingConvention());

        // ── EF Interceptors ───────────────────────────────────────────────────
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        services.AddScoped<DomainEventDispatchInterceptor>();

        // ── Repositories & Unit of Work ───────────────────────────────────────
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IFarmerSessionRepository, FarmerSessionRepository>();
        services.AddScoped<IKnowledgeEntryRepository, KnowledgeEntryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        // ── Redis Cache ───────────────────────────────────────────────────────
        services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.SectionName));
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(
                configuration.GetSection(RedisOptions.SectionName)["ConnectionString"]
                ?? "localhost:6379"));
        services.AddScoped<ICacheService, RedisCacheService>();

        // ── JWT Authentication ────────────────────────────────────────────────
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        var jwtOpts = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()!;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOpts.Issuer,
                    ValidAudience = jwtOpts.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOpts.SecretKey)),
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        services.AddAuthorization(opts =>
        {
            opts.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            opts.AddPolicy("AgronomiystOrAdmin", policy => policy.RequireRole("Admin", "Agronom"));
        });

        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IRefreshTokenStore, PostgresRefreshTokenStore>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // ── Claude AI ─────────────────────────────────────────────────────────
        services.Configure<ClaudeOptions>(configuration.GetSection(ClaudeOptions.SectionName));
        services.AddScoped<IClaudeAiService, ClaudeAiService>();
        services.AddScoped<IRagPipelineService, RagPipelineService>();

        // ── Embedding Service (Voyage AI) ─────────────────────────────────────
        services.Configure<EmbeddingOptions>(configuration.GetSection(EmbeddingOptions.SectionName));
        services.AddHttpClient<IEmbeddingService, ClaudeEmbeddingService>((sp, client) =>
        {
            var opts = configuration.GetSection(EmbeddingOptions.SectionName).Get<EmbeddingOptions>()!;
            client.BaseAddress = new Uri(opts.BaseUrl);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {opts.ApiKey}");
        });

        // ── Qdrant Vector Database ────────────────────────────────────────────
        services.Configure<QdrantOptions>(configuration.GetSection(QdrantOptions.SectionName));
        services.AddSingleton<QdrantClient>(sp =>
        {
            var opts = configuration.GetSection(QdrantOptions.SectionName).Get<QdrantOptions>()!;
            return new QdrantClient(opts.Host, opts.Port, apiKey: opts.ApiKey);
        });
        services.AddScoped<IVectorSearchService, QdrantVectorSearchService>();
        services.AddHostedService<QdrantCollectionInitializer>();

        // ── Telegram Bot ──────────────────────────────────────────────────────
        services.Configure<TelegramBotOptions>(configuration.GetSection(TelegramBotOptions.SectionName));
        services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var token = configuration[$"{TelegramBotOptions.SectionName}:BotToken"]!;
            return new TelegramBotClient(token);
        });
        services.AddScoped<ITelegramNotificationService, TelegramBotService>();
        services.AddScoped<TelegramUpdateHandler>();
        services.AddHostedService<TelegramBotHostedService>();

        // ── Current User / DateTime Services ─────────────────────────────────
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // ── Background Jobs ───────────────────────────────────────────────────
        services.AddHostedService<KnowledgeIndexingJob>();
        services.AddHostedService<SessionCleanupJob>();

        return services;
    }
}
