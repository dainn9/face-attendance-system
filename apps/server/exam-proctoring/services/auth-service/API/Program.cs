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

var builder = WebApplication.CreateBuilder(args);

// =============================
// Add Layers
// =============================
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();


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
            }
        };
    });

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
