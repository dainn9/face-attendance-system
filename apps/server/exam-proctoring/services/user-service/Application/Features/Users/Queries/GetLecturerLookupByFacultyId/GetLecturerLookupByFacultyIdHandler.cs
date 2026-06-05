using MediatR;
using user_service.Application.Abstractions.Persistence;
using user_service.Application.Contracts;

namespace user_service.Application.Features.Users.Queries.GetLecturerLookupByFacultyId
{
    public class GetLecturerLookupByFacultyIdHandler : IRequestHandler<GetLecturerLookupByFacultyIdQuery, IReadOnlyList<UserLookupDto>>
    {
        private readonly IUserReadRepository _userReadRepository;

        public GetLecturerLookupByFacultyIdHandler(IUserReadRepository userReadRepository) =>
            _userReadRepository = userReadRepository;

        public Task<IReadOnlyList<UserLookupDto>> Handle(GetLecturerLookupByFacultyIdQuery request, CancellationToken cancellationToken)
        => _userReadRepository.GetLecturerLookupByFacultyIdAsync(request.FacultyId, request.Keyword, cancellationToken);
    }
}