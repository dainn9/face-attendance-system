using attendance_service.Application.Abstractions.Persistence;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Time;
using MediatR;

namespace attendance_service.Application.Features.Attendances.Commands.CloseAttendanceSession
{
    public class CloseAttendanceSessionHandler : IRequestHandler<CloseAttendanceSessionCommand>
    {
        private readonly IAttendanceSessionRepository _attendanceSessionRepository;
        private readonly ICourseSectionReadRepository _courseSectionReadRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClock _clock;

        public CloseAttendanceSessionHandler(IAttendanceSessionRepository attendanceSessionRepository, ICourseSectionReadRepository courseSectionReadRepository, IUnitOfWork unitOfWork, IClock clock)
        {
            _attendanceSessionRepository = attendanceSessionRepository;
            _courseSectionReadRepository = courseSectionReadRepository;
            _unitOfWork = unitOfWork;
            _clock = clock;
        }

        public async Task Handle(CloseAttendanceSessionCommand request, CancellationToken cancellationToken)
        {
            var attendanceSession = await _attendanceSessionRepository.GetByIdWithRecordsAsync(request.AttendanceSessionId, cancellationToken)
                ?? throw new EntityNotFoundException("Attendance session", request.AttendanceSessionId);

            var lectureId = await _courseSectionReadRepository.GetLecturerIdAsync(attendanceSession.CourseSectionId, cancellationToken)
                ?? throw new EntityNotFoundException("Course section", attendanceSession.CourseSectionId);

            if (lectureId != request.LecturerId)
                throw new ForbiddenException("You are not authorized to close this attendance session.");

            var studentIds = await _courseSectionReadRepository.GetEnrollmentStudentIdsAsync(attendanceSession.CourseSectionId, cancellationToken);

            var now = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(
                _clock.UtcNow,
                "SE Asia Standard Time");
            attendanceSession.Close(now);
            attendanceSession.MarkAbsentStudents(studentIds, now);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}