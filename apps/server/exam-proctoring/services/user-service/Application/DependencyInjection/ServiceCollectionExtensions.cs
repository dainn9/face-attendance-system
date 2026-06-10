using BuildingBlocks.Behaviors;
using FluentValidation;
using MediatR;

namespace user_service.Application.DependencyInjection
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

            // Tự động scan và đăng ký tất cả validator trong assembly
            services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);

            return services;
        }
    }
}