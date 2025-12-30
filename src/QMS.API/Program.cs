using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Serilog;
using Asp.Versioning;
using QMS.Infrastructure.Data;
using QMS.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// ========== LOGGING ==========
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ========== SERVICES CONFIGURATION ==========

// Controllers
builder.Services.AddControllers();

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"));
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Database Context
builder.Services.AddDbContext<QMSDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("QMS.Infrastructure")));

// Authentication & Authorization
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? ""))
    };
});

builder.Services.AddAuthorization();

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

// Response Compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

// Caching
builder.Services.AddMemoryCache();

// Health Checks
builder.Services.AddHealthChecks()
    .AddSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection") ?? "",
        name: "database",
        tags: new[] { "db", "sql", "sqlserver" });

// SignalR
builder.Services.AddSignalR();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "QMS RESTful API",
        Version = "v1",
        Description = "Queue Management System API Documentation",
        Contact = new OpenApiContact
        {
            Name = "Your Company",
            Email = "support@yourcompany.com"
        }
    });

    // JWT Authentication in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ========== MIDDLEWARE PIPELINE ==========

var app = builder.Build();

// Exception Handling
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Request Logging
app.UseSerilogRequestLogging();

// Swagger (Development & Staging)
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "QMS API V1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}

// HTTPS Redirection
app.UseHttpsRedirection();

// Response Compression
app.UseResponseCompression();

// CORS
app.UseCors(app.Environment.IsDevelopment() ? "AllowAll" : "Production");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Health Checks
app.MapHealthChecks("/health");

// Controllers
app.MapControllers();

// SignalR Hubs
// app.MapHub<QueueHub>("/hubs/queue");

// Run
try
{
    Log.Information("Starting QMS API...");
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