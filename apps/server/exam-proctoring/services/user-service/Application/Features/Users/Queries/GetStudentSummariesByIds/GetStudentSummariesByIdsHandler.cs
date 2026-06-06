using MediatR;
using user_service.Application.Abstractions.Persistence;
using user_service.Application.Contracts;

namespace user_service.Application.Features.Users.Queries.GetStudentSummariesByIds
{
    public class GetStudentSummariesByIdsHandler : IRequestHandler<GetStudentSummariesByIdsQuery, Dictionary<Guid, StudentSummaryDto>>
    {
        private readonly IUserReadRepository _userReadRepository;

        public GetStudentSummariesByIdsHandler(IUserReadRepository userReadRepository) =>
            _userReadRepository = userReadRepository;

        public Task<Dictionary<Guid, StudentSummaryDto>> Handle(GetStudentSummariesByIdsQuery request, CancellationToken cancellationToken)
        => _userReadRepository.GetStudentSummariesByIdsAsync(
            request.StudentIds,
            cancellationToken
        );
    }
}