using auth_service.Application.Abstractions.Persistence;
using auth_service.Application.Contracts;
using BuildingBlocks.Exceptions;
using MediatR;

namespace auth_service.Application.Features.Auth.Queries.GetMe
{
    public class GetMeHandler : IRequestHandler<GetMeQuery, MeResponse>
    {
        private readonly IUserRepository _userRepository;

        public GetMeHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<MeResponse> Handle(GetMeQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
                ?? throw new EntityNotFoundException("User not found");

            return new MeResponse(
                Id: user.Id,
                Email: user.Email.Value,
                Role: user.Role.ToString(),
                IsActive: user.IsActive
            );
        }
    }
}