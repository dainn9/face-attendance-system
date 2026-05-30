// using BuildingBlocks.Exceptions;
// using MediatR;
// using user_service.Application.Abstractions.Persistence;
// using user_service.Application.Contracts;

// namespace user_service.Application.Features.Users.Queries.GetUserProfile
// {
//     public class GetUserProfileHandler : IRequestHandler<GetUserProfileQuery, UserDto>
//     {
//         private readonly IUserReadRepository _userRepository;

//         public GetUserProfileHandler(IUserReadRepository userRepository) => _userRepository = userRepository;

//         public async Task<UserDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
//         => await _userRepository.GetProfileByIdAsync(request.UserId, cancellationToken)
//         ?? throw new EntityNotFoundException("User not found");

//     }
// }