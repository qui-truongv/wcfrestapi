using Microsoft.EntityFrameworkCore;
using QMS.Application.Interfaces;
using QMS.Application.Mappings;
using QMS.Application.Services;
using QMS.Core.Interfaces;
using QMS.Core.Interfaces.Repositories;
using QMS.Infrastructure.Data;
using QMS.Infrastructure.Repositories;

namespace QMS.API.Extensions;

/// <summary>
/// Service Collection Extensions for Dependency Injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add all QMS services
    /// </summary>
    public static IServiceCollection AddQMSServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database Context
        services.AddDbContext<QMSDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("QMS.Infrastructure")));

        // AutoMapper
        services.AddAutoMapper(typeof(QueueMappingProfile).Assembly);

        // Unit of Work & Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IQueueRepository, QueueRepository>();
        services.AddScoped<IQueueItemRepository, QueueItemRepository>();
        services.AddScoped<IScreenRepository, ScreenRepository>();
        services.AddScoped<IKioskRepository, KioskRepository>();
        services.AddScoped<IParameterRepository, ParameterRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();

        // Application Services
        services.AddScoped<IQueueService, QueueService>();
        services.AddScoped<IScreenService, ScreenService>();
        services.AddScoped<IKioskService, KioskService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IQMSHelperService, QMSHelperService>();

        // Cache Service - Singleton for in-memory cache
        services.AddSingleton<IQMSCacheService, QMSCacheService>();

        return services;
    }

    /// <summary>
    /// Initialize QMS cache on startup
    /// </summary>
    public static async Task InitializeQMSCacheAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var cacheService = scope.ServiceProvider.GetRequiredService<IQMSCacheService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<QMSCacheService>>();

        try
        {
            logger.LogInformation("Initializing QMS cache on startup...");
            await cacheService.InitializeCacheAsync();
            logger.LogInformation("QMS cache initialized successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize QMS cache on startup");
            // Don't throw - allow application to start even if cache fails
        }
    }
}

/// <summary>
/// Updated Program.cs configuration
/// </summary>
public static class ProgramConfiguration
{
    public static void ConfigureQMSServices(WebApplicationBuilder builder)
    {
        // Add QMS Services
        builder.Services.AddQMSServices(builder.Configuration);

        // Memory Cache
        builder.Services.AddMemoryCache();

        // Controllers
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null; // Keep PascalCase
                options.JsonSerializerOptions.ReferenceHandler =
                    System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });

        // API Versioning
        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = Asp.Versioning.ApiVersionReader.Combine(
                new Asp.Versioning.UrlSegmentApiVersionReader(),
                new Asp.Versioning.HeaderApiVersionReader("X-Api-Version"));
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        // Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "QMS RESTful API",
                Version = "v1",
                Description = "Queue Management System REST API - Migrated from WCF",
                Contact = new Microsoft.OpenApi.Models.OpenApiContact
                {
                    Name = "QMS Development Team"
                }
            });

            // Enable XML comments
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }

            // JWT Authentication
            c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme",
                Name = "Authorization",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
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
                    Array.Empty<string>()
                }
            });
        });

        // CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });

            options.AddPolicy("Production", policy =>
            {
                var allowedOrigins = builder.Configuration
                    .GetSection("Cors:AllowedOrigins")
                    .Get<string[]>() ?? Array.Empty<string>();

                policy.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });

        // Health Checks
        builder.Services.AddHealthChecks()
            .AddSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection") ?? "",
                name: "database",
                tags: new[] { "db", "sql" });

        // SignalR
        builder.Services.AddSignalR();
    }

    public static async Task ConfigureQMSMiddleware(WebApplication app)
    {
        // Exception Handling
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "QMS API V1");
                c.RoutePrefix = string.Empty; // Swagger at root
            });
        }

        app.UseHttpsRedirection();

        // CORS
        app.UseCors(app.Environment.IsDevelopment() ? "AllowAll" : "Production");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHealthChecks("/health");

        // SignalR Hubs (when implemented)
        // app.MapHub<QueueHub>("/hubs/queue");

        // Initialize Cache
        await app.Services.InitializeQMSCacheAsync();
    }
}

/// <summary>
/// Example Updated Program.cs
/// </summary>
/*
using QMS.API.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Configure QMS Services
ProgramConfiguration.ConfigureQMSServices(builder);

var app = builder.Build();

// Configure Middleware
await ProgramConfiguration.ConfigureQMSMiddleware(app);

try
{
    Log.Information("Starting QMS RESTful API...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}
*/