using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Application.Contracts;
using MediatR;

namespace attendance_service.Application.Features.Subjects.Queries.GetSubjectLookup
{
    public class GetSubjectLookupHandler : IRequestHandler<GetSubjectLookupQuery, IReadOnlyList<SubjectLookupDto>>
    {
        private readonly ISubjectReadRepository _subjectReadRepository;

        public GetSubjectLookupHandler(ISubjectReadRepository subjectReadRepository) =>
            _subjectReadRepository = subjectReadRepository;

        public Task<IReadOnlyList<SubjectLookupDto>> Handle(GetSubjectLookupQuery request, CancellationToken cancellationToken)
        => _subjectReadRepository.GetSubjectLookupAsync(request.Keyword, cancellationToken);
    }
}