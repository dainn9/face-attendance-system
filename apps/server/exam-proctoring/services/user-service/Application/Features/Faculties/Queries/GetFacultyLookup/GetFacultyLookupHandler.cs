using MediatR;
using user_service.Application.Abstractions.Persistence;
using user_service.Application.Contracts.Faculties;

namespace user_service.Application.Features.Faculties.Queries.GetFacultyLookup
{
    public class GetFacultyLookupHandler : IRequestHandler<GetFacultyLookupQuery, IReadOnlyList<FacultyLookupDto>>
    {
        private readonly IFacultyReadRepository _facultyReadRepository;

        public GetFacultyLookupHandler(IFacultyReadRepository facultyReadRepository) =>
            _facultyReadRepository = facultyReadRepository;

        public Task<IReadOnlyList<FacultyLookupDto>> Handle(GetFacultyLookupQuery request, CancellationToken cancellationToken)
        => _facultyReadRepository.GetFacultyLookupAsync(cancellationToken);
    }
}