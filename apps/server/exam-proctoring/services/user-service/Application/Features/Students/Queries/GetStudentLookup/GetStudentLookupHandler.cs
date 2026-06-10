using MediatR;
using user_service.Application.Abstractions.Persistence;
using user_service.Application.Contracts.Students;

namespace user_service.Application.Features.Students.Queries.GetStudentLookup
{
    public class GetStudentLookupHandler : IRequestHandler<GetStudentLookupQuery, IReadOnlyList<StudentLookupDto>>
    {
        private readonly IStudentReadRepository _studentReadRepository;

        public GetStudentLookupHandler(IStudentReadRepository studentReadRepository) =>
            _studentReadRepository = studentReadRepository;

        public Task<IReadOnlyList<StudentLookupDto>> Handle(GetStudentLookupQuery request, CancellationToken cancellationToken)
        => _studentReadRepository.GetStudentLookupAsync(request.Keyword, cancellationToken);
    }
}