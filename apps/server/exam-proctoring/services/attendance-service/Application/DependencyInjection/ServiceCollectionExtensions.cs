using attendance_service.Application.Abstractions.Services;
using attendance_service.Application.Services;
using BuildingBlocks.Behaviors;
using FluentValidation;
using MediatR;

namespace attendance_service.Application.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // ==========================
            // MediatR
            // ==========================

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly);

                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);

            // ==========================
            // Other services
            // ==========================
            services.AddScoped<IAttendanceService, AttendanceService>();

            return services;
        }
    }
}