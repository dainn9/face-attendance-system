using api_gateway.Clients;
using api_gateway.Middleware;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Results;
using BuildingBlocks.Security.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
// Add services to the container.

// =============================
// Clients
// =============================
static void ConfigureInternalClient(
    IServiceProvider sp,
    HttpClient client,
    string serviceKey)
{
    var config = sp.GetRequiredService<IConfiguration>();

    client.BaseAddress = new Uri(
        config[$"Services:{serviceKey}:BaseUrl"]!);

    client.DefaultRequestHeaders.Add(
        "X-Internal-Api-Key",
        config["InternalAuth:ApiKey"]!);
}

builder.Services.AddHttpClient<UserClient>(
    (sp, client) =>
        ConfigureInternalClient(
            sp,
            client,
            "UserService"));

builder.Services.AddHttpClient<AuthClient>(
    (sp, client) =>
        ConfigureInternalClient(
            sp,
            client,
            "AuthService"));

builder.Services.AddHttpClient<AttendanceClient>(
    (sp, client) =>
        ConfigureInternalClient(
            sp,
            client,
            "AttendanceService"));

builder.Services.AddHttpClient<FaceClient>(
    (sp, client) =>
        ConfigureInternalClient(
            sp,
            client,
            "FaceService"));

builder.Services.AddControllers()
.AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =============================
// Authentication / Authorization
// =============================
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();

if (jwtSettings == null)
    throw new ConfigurationException("Missing JWT configuration in appsettings.json");

if (string.IsNullOrWhiteSpace(jwtSettings.PublicKeyPath))
    throw new ConfigurationException("JWT public key path is missing.");

if (!File.Exists(jwtSettings.PublicKeyPath))
    throw new ConfigurationException($"JWT public key file not found: {jwtSettings.PublicKeyPath}");

var publicPem = await File.ReadAllTextAsync(jwtSettings.PublicKeyPath);
using var rsa = RSA.Create();
rsa.ImportFromPem(publicPem);

// CORS Configuration

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
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
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
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
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                });
        }
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors("AllowFrontend");

// =============================
// Middleware
// =============================
app.UseMiddleware<GatewayExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();
app.MapControllers();

app.Run();
