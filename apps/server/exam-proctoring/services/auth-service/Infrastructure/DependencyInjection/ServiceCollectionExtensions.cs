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

            // ==========================
            // Password Hasher            
            // ==========================
            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

            // ==========================
            // Seeders
            // ==========================
            services.AddScoped<IUserSeeder, UserSeeder>();

            return services;
        }
    }
}