using FluentValidation;
using MediatR;

namespace BuildingBlocks.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            // Không có validator nào → bỏ qua, đi tiếp
            if (!_validators.Any()) return await next();

            // Tạo context chứa data cần validate
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, ct))
            );

            // Chạy tất cả validator, gom lại các lỗi
            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            // Có lỗi → throw, không cho vào Handler
            if (failures.Any())
                throw new ValidationException(failures);

            // Không có lỗi → cho đi tiếp vào Handler
            return await next();
        }
    }
}

