using MediatR;
using user_service.Application.Abstractions.Persistence;
using user_service.Application.Contracts.Faculties;

namespace user_service.Application.Features.Faculties.Queries.GetFaculties
{
    public record GetFacultiesHandler : IRequestHandler<GetFacultiesQuery, IReadOnlyList<FacultyDto>>
    {
        private readonly IFacultyReadRepository _facultyReadRepository;
        private readonly IUserReadRepository _userReadRepository;

        public GetFacultiesHandler(IFacultyReadRepository facultyReadRepository, IUserReadRepository userReadRepository)
        {
            _facultyReadRepository = facultyReadRepository;
            _userReadRepository = userReadRepository;
        }

        public async Task<IReadOnlyList<FacultyDto>> Handle(GetFacultiesQuery request, CancellationToken cancellationToken)
        {
            var faculties = await _facultyReadRepository.GetFacultiesAsync(cancellationToken);

            var studentCounts = await _userReadRepository.GetStudentCountByMajorsAsync(cancellationToken);

            var lecturerCounts = await _userReadRepository.GetLecturerCountByFacultyAsync(cancellationToken);

            faculties = faculties
            .Select(f =>
            {
                var majorsWithCounts = f.Majors
                    .Select(m => m with
                    {
                        StudentCount = studentCounts.GetValueOrDefault(m.Id)
                    })
                    .OrderByDescending(m => m.StudentCount)
                    .ToArray();

                return f with
                {
                    Majors = majorsWithCounts.Take(3).ToArray(),
                    StudentCount = majorsWithCounts.Sum(m => m.StudentCount),
                    LecturerCount = lecturerCounts.GetValueOrDefault(f.Id)
                };
            })
            .ToList();

            return faculties;
        }
    }

    public record GetFacultiesQuery : IRequest<IReadOnlyList<FacultyDto>>;
}