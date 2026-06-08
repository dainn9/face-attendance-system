using MediatR;
using user_service.Application.Abstractions.Persistence;
using user_service.Application.Contracts.Students;

namespace user_service.Application.Features.Students.Queries.GetStudentBasicsByIds
{
    public class GetStudentBasicsByIdsHandler : IRequestHandler<GetStudentBasicsByIdsQuery, Dictionary<Guid, StudentBasicDto>>
    {
        private readonly IStudentReadRepository _studentReadRepository;

        public GetStudentBasicsByIdsHandler(IStudentReadRepository studentReadRepository)
        {
            _studentReadRepository = studentReadRepository;
        }

        public Task<Dictionary<Guid, StudentBasicDto>> Handle(GetStudentBasicsByIdsQuery request, CancellationToken cancellationToken)
        => _studentReadRepository.GetStudentBasicsByIdsAsync(request.StudentIds, cancellationToken);
    }
}