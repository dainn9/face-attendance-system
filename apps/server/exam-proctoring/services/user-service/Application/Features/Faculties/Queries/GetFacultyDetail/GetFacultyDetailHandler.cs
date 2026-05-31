using BuildingBlocks.Exceptions;
using MediatR;
using user_service.Application.Abstractions.Persistence;
using user_service.Application.Contracts;

namespace user_service.Application.Features.Faculties.Queries.GetFacultyDetail
{
    public class GetFacultyDetailHandler : IRequestHandler<GetFacultyDetailQuery, FacultyDetailDto>
    {
        private readonly IFacultyReadRepository _facultyReadRepository;
        private readonly IUserReadRepository _userReadRepository;

        public GetFacultyDetailHandler(IFacultyReadRepository facultyReadRepository, IUserReadRepository userReadRepository)
        {
            _facultyReadRepository = facultyReadRepository;
            _userReadRepository = userReadRepository;
        }

        public async Task<FacultyDetailDto> Handle(GetFacultyDetailQuery request, CancellationToken ct)
        {
            var faculty = await _facultyReadRepository.GetFacultyByIdAsync(request.Id, ct)
                ?? throw new EntityNotFoundException("Faculty", request.Id);

            var studentCounts = await _userReadRepository.GetStudentCountByFacultyIdAsync(request.Id, ct);
            var lecturers = await _userReadRepository.GetLecturersByFacultyIdAsync(request.Id, ct);

            faculty = faculty with
            {
                Majors = faculty.Majors
                    .Select(m => m with
                    {
                        StudentCount = studentCounts.GetValueOrDefault(m.Id)
                    })
                    .ToArray(),
                StudentCount = studentCounts.Values.Sum(),
                LecturerCount = lecturers.Count
            };

            var facultyDetail = new FacultyDetailDto(
                faculty.Id,
                faculty.Name,
                faculty.Code,
                faculty.Majors.Length,
                faculty.StudentCount,
                faculty.LecturerCount,
                faculty.Majors,
                lecturers.ToArray()
            );

            return facultyDetail;
        }
    }

    public record GetFacultyDetailQuery(Guid Id) : IRequest<FacultyDetailDto>;
}