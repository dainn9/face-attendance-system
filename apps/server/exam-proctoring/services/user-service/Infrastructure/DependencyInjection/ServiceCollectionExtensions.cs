using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Time;
using Microsoft.EntityFrameworkCore;
using user_service.Application.Abstractions.Persistence;
using user_service.Infrastructure.Persistence;
using user_service.Infrastructure.Persistence.Repositories;

namespace user_service.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // ==========================
            // Database-MySQL
            // ==========================

            var connectionString = configuration.GetConnectionString("Default");

            services.AddDbContext<UserDbContext>(options =>
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
            // Repositories
            // ===========================
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IFacultyRepository, FacultyRepository>();

            // ==========================
            // Read Repositories
            // ==========================
            services.AddScoped<IUserReadRepository, UserReadRepository>();
            services.AddScoped<IFacultyReadRepository, FacultyReadRepository>();
            services.AddScoped<IStudentReadRepository, StudentReadRepository>();

            // ==========================
            // Unit of Work            
            // ==========================
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ==========================
            // Utilities
            // ==========================
            services.AddSingleton<IClock, SystemClock>();

            return services;
        }
    }
}