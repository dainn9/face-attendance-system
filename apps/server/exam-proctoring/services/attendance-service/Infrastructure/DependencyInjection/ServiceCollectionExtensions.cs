using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Infrastructure.Persistence;
using attendance_service.Infrastructure.Persistence.Repositories;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Time;
using Microsoft.EntityFrameworkCore;

namespace attendance_service.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // ==========================
            // Database-MySQL
            // ==========================
            var connectionString = configuration.GetConnectionString("Default");

            services.AddDbContext<AttendanceDbContext>(options =>
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
            // ==========================
            services.AddScoped<ISubjectRepository, SubjectRepository>();
            services.AddScoped<ISubjectReadRepository, SubjectReadRepository>();
            services.AddScoped<ICourseSectionReadRepository, CourseSectionReadRepository>();

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