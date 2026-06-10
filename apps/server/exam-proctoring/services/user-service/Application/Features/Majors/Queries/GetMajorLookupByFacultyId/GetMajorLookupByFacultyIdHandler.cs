using MediatR;
using user_service.Application.Abstractions.Persistence;
using user_service.Application.Contracts.Majors;

namespace user_service.Application.Features.Majors.Queries.GetMajorLookupByFacultyId
{
    public class GetMajorLookupByFacultyIdHandler : IRequestHandler<GetMajorLookupByFacultyIdQuery, IReadOnlyList<MajorLookupDto>>
    {
        private readonly IFacultyReadRepository _facultyReadRepository;

        public GetMajorLookupByFacultyIdHandler(IFacultyReadRepository facultyReadRepository) =>
            _facultyReadRepository = facultyReadRepository;

        public Task<IReadOnlyList<MajorLookupDto>> Handle(GetMajorLookupByFacultyIdQuery request, CancellationToken cancellationToken)
        => _facultyReadRepository.GetMajorLookupByFacultyIdAsync(request.FacultyId, cancellationToken);
    }
}