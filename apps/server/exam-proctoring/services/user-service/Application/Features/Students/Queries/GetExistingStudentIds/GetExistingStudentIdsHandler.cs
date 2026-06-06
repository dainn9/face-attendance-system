using MediatR;
using user_service.Application.Abstractions.Persistence;

namespace user_service.Application.Features.Students.Queries.GetExistingStudentIds
{
    public class GetExistingStudentIdsHandler : IRequestHandler<GetExistingStudentIdsQuery, IReadOnlyList<Guid>>
    {
        private readonly IStudentReadRepository _studentReadRepository;

        public GetExistingStudentIdsHandler(IStudentReadRepository studentReadRepository)
        {
            _studentReadRepository = studentReadRepository;
        }

        public async Task<IReadOnlyList<Guid>> Handle(GetExistingStudentIdsQuery request, CancellationToken cancellationToken)
        => await _studentReadRepository.GetExistingStudentIdsAsync(request.StudentIds, cancellationToken);
    }
}