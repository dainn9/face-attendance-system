using auth_service.Application.Abstractions.Caching;
using auth_service.Application.Abstractions.Persistence;
using auth_service.Application.Abstractions.Security;
using auth_service.Application.Abstractions.Seed;
using auth_service.Infrastructure.Caching;
using auth_service.Infrastructure.Persistence;
using auth_service.Infrastructure.Persistence.Repositories;
using auth_service.Infrastructure.Security.Jwt;
using auth_service.Infrastructure.Security.Password;
using auth_service.Infrastructure.Seed;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Time;
using Microsoft.EntityFrameworkCore;

namespace auth_service.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // ==========================
            // Database-MySQL
            // ==========================
            var connectionString = configuration.GetConnectionString("Default");

            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    mysql =>
                    {
                        mysql.EnableRetryOnFailure(5);
                    });
            });

            // ==========================
            // JWT
            // ==========================
            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<IJwtKeyProvider, JwtKeyProvider>();

            // ==========================
            // Caching-Redis
            // ==========================
            services.Configure<RedisOptions>(configuration.GetSection("Redis"));
            services.AddSingleton<RedisConnectionFactory>();
            services.AddScoped<IRefreshTokenStore, RedisRefreshTokenStore>();


            // ==========================
            // Repositories
            // ==========================
            services.AddScoped<IUserRepository, UserRepository>();
            // services.AddScoped<IOutboxRepository, OutboxRepository>();

            // ==========================
            // Password Hasher            
            // ==========================
            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

            // ==========================
            // Seeders
            // ==========================
            services.AddScoped<IUserSeeder, UserSeeder>();

            // ==========================
            // System
            // ==========================
            services.AddScoped<IClock, SystemClock>();

            // ==========================
            // Clients
            // ==========================
            // services.AddHttpClient<IUserInternalClient, UserInternalClient>((sp, client) =>
            // {
            //     var config = sp.GetRequiredService<IConfiguration>();

            //     client.BaseAddress = new Uri(config["Services:UserService:BaseUrl"]!);
            //     client.DefaultRequestHeaders.Add(
            //         "X-Internal-Api-Key",
            //         config["InternalAuth:ApiKey"]!
            //     );
            // });

            // ==========================
            // Unit of Work
            // ==========================
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ==========================
            // Hosted Services
            // ==========================
            // services.AddHostedService<OutboxProcessor>();

            return services;
        }
    }
}
