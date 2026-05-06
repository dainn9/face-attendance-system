using auth_service.Application.Abstractions.Seed;
using auth_service.Application.DependencyInjection;
using auth_service.Infrastructure.DependencyInjection;
using auth_service.Infrastructure.Persistence;
using auth_service.Infrastructure.Security.Jwt;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// =============================
// Add Layers
// =============================
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
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
var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
if (jwtOptions == null)
    throw new ConfigurationException("Missing JWT configuration in appsettings.json");

if (string.IsNullOrWhiteSpace(jwtOptions.PrivateKeyPath))
    throw new ConfigurationException("JWT private key path is missing.");

if (!File.Exists(jwtOptions.PrivateKeyPath))
    throw new ConfigurationException($"JWT private key file not found: {jwtOptions.PrivateKeyPath}");

var privatePem = await File.ReadAllTextAsync(jwtOptions.PrivateKeyPath);
using var rsa = RSA.Create();
rsa.ImportFromPem(privatePem);

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

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

            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
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
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    var result = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        errorCode = ErrorCodes.Token_Expired,
                        // message = "Your session has expired. Please log in again."
                    });
                    return context.Response.WriteAsync(result);
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();
app.UseCors("AllowFrontend");

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
    var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
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

        // Seed default users
        var seeder = scope.ServiceProvider.GetRequiredService<IUserSeeder>();
        await seeder.SeedAsync();
    }
}

app.Run();
