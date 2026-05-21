using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Middleware;
using BuildingBlocks.Results;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using user_service.API.Auth;
using user_service.Application.DependencyInjection;
using user_service.Infrastructure.DependencyInjection;
using user_service.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// =============================
// Add Layers
// =============================
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition =
                JsonIgnoreCondition.WhenWritingNull;
        });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =============================
// Authentication / Authorization
// =============================
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<BuildingBlocks.Security.Jwt.JwtSettings>();

if (jwtSettings == null)
    throw new ConfigurationException("Missing JWT configuration in appsettings.json");

if (string.IsNullOrWhiteSpace(jwtSettings.PublicKeyPath))
    throw new ConfigurationException("JWT public key path is missing.");

if (!File.Exists(jwtSettings.PublicKeyPath))
    throw new ConfigurationException($"JWT public key file not found: {jwtSettings.PublicKeyPath}");

var publicPem = await File.ReadAllTextAsync(jwtSettings.PublicKeyPath);
using var rsa = RSA.Create();
rsa.ImportFromPem(publicPem);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new RsaSecurityKey(rsa.ExportParameters(false)),
            ClockSkew = TimeSpan.FromSeconds(30),
        };

        // Support cookie-based token delivery for browser clients
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.TryGetValue("access", out var token) && !string.IsNullOrWhiteSpace(token))
                    context.Token = token;
                return Task.CompletedTask;
            },

            OnAuthenticationFailed = context =>
            {
                if (context.Exception is not SecurityTokenExpiredException)
                    return Task.CompletedTask;

                context.NoResult();

                var traceId = context.HttpContext.TraceIdentifier;

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var response = new ApiResponse<object>
                {
                    Success = false,
                    Message = "Your session has expired. Please log in again.",
                    ErrorCode = ErrorCodes.Token_Expired,
                    TraceId = traceId
                };

                return context.Response.WriteAsJsonAsync(response,
                    new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    });
            },

            OnChallenge = context =>
            {
                context.HandleResponse();

                var traceId = context.HttpContext.TraceIdentifier;

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var hasRefreshToken = context.Request.Cookies.ContainsKey("refresh");

                var response = new ApiResponse<object>
                {
                    Success = false,
                    Message = hasRefreshToken ? "Your session has expired. Please log in again." : "Unauthorized",
                    ErrorCode = hasRefreshToken ? ErrorCodes.Token_Expired : ErrorCodes.Unauthorized,
                    TraceId = traceId
                };

                return context.Response.WriteAsJsonAsync(response,
                    new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    });
            }
        };
    })
    .AddScheme<AuthenticationSchemeOptions, InternalAuthenticationHandler>(
        InternalAuthenticationHandler.SchemeName, options => { });

builder.Services.AddAuthorization();

var app = builder.Build();

// =============================
// Middleware
// =============================
app.UseGlobalExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Database Migration & Seed
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    if (app.Environment.IsDevelopment())
    {
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

        if (pendingMigrations.Any())
        {
            Console.WriteLine("Applying migrations...");
            await context.Database.MigrateAsync();
            Console.WriteLine("Migrations applied successfully.");
        }
        else
        {
            Console.WriteLine("Database is up to date.");
        }
    }
}

app.Run();
