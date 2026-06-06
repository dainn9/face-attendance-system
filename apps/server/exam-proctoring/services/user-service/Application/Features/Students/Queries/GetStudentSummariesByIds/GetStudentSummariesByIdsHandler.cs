using MediatR;
using user_service.Application.Abstractions.Persistence;
using user_service.Application.Contracts.Students;

namespace user_service.Application.Features.Students.Queries.GetStudentSummariesByIds
{
    public class GetStudentSummariesByIdsHandler : IRequestHandler<GetStudentSummariesByIdsQuery, Dictionary<Guid, StudentSummaryDto>>
    {
        private readonly IStudentReadRepository _studentReadRepository;

        public GetStudentSummariesByIdsHandler(IStudentReadRepository studentReadRepository) =>
            _studentReadRepository = studentReadRepository;

        public Task<Dictionary<Guid, StudentSummaryDto>> Handle(GetStudentSummariesByIdsQuery request, CancellationToken cancellationToken)
        => _studentReadRepository.GetStudentSummariesByIdsAsync(
            request.StudentIds,
            cancellationToken
        );
    }
}