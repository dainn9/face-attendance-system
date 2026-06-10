using attendance_service.Application.Abstractions.Persistence;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Time;
using MediatR;

namespace attendance_service.Application.Features.Attendances.Commands.CheckInAttendance
{
    public class CheckInAttendanceHandler : IRequestHandler<CheckInAttendanceCommand>
    {
        private readonly IAttendanceSessionRepository _attendanceSessionRepository;
        private readonly ICourseSectionReadRepository _courseSectionReadRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClock _clock;

        public CheckInAttendanceHandler(IAttendanceSessionRepository attendanceSessionRepository, ICourseSectionReadRepository courseSectionReadRepository, IUnitOfWork unitOfWork, IClock clock)
        {
            _attendanceSessionRepository = attendanceSessionRepository;
            _courseSectionReadRepository = courseSectionReadRepository;
            _unitOfWork = unitOfWork;
            _clock = clock;
        }

        public async Task Handle(CheckInAttendanceCommand request, CancellationToken cancellationToken)
        {
            var attendanceSession = await _attendanceSessionRepository.GetByIdWithRecordsAsync(request.AttendanceSessionId, cancellationToken)
                ?? throw new EntityNotFoundException("Attendance session", request.AttendanceSessionId);

            if (!await _courseSectionReadRepository.IsStudentEnrolledAsync(attendanceSession.CourseSectionId, request.StudentId, cancellationToken))
                throw new ForbiddenException("Student is not enrolled in this course section.");

            var now = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(
                _clock.UtcNow,
                "SE Asia Standard Time");

            attendanceSession.CheckInStudent(request.StudentId, now, request.Confidence);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}