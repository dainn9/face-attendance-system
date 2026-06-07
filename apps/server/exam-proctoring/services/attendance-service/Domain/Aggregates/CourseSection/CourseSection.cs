using attendance_service.Domain.Enums;
using BuildingBlocks.Exceptions;
using SharedKernel.Core;

namespace attendance_service.Domain.Aggregates.CourseSection
{
    public class CourseSection : AggregateRoot<Guid>
    {
        public Guid SubjectId { get; private set; }
        public string CourseSectionCode { get; private set; } = null!;
        public Semester Semester { get; private set; }
        public string AcademicYear { get; private set; } = null!;
        public Guid LecturerId { get; private set; }
        public int MaxCapacity { get; private set; }
        public bool IsActive { get; private set; }

        private readonly List<Enrollment> _enrollments = new();
        public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

        private readonly List<Schedule> _schedules = new();
        public IReadOnlyCollection<Schedule> Schedules => _schedules.AsReadOnly();

        private CourseSection() { }

        public static CourseSection Create(Guid subjectId, string courseSectionCode, Semester semester, string academicYear, Guid lecturerId, int maxCapacity, DateTime now)
        {
            if (subjectId == Guid.Empty)
                throw new BusinessRuleViolationException("Subject ID cannot be empty.", ErrorCodes.InvalidCourseSectionData);

            if (string.IsNullOrWhiteSpace(courseSectionCode))
                throw new BusinessRuleViolationException("Course section code cannot be empty.", ErrorCodes.InvalidCourseSectionData);

            if (string.IsNullOrWhiteSpace(academicYear))
                throw new BusinessRuleViolationException("Academic year cannot be empty.", ErrorCodes.InvalidCourseSectionData);

            if (lecturerId == Guid.Empty)
                throw new BusinessRuleViolationException("Lecturer ID cannot be empty.", ErrorCodes.InvalidCourseSectionData);

            if (maxCapacity <= 0)
                throw new BusinessRuleViolationException("Maximum capacity must be greater than zero.", ErrorCodes.InvalidCourseSectionData);

            var courseSection = new CourseSection
            {
                Id = Guid.NewGuid(),
                SubjectId = subjectId,
                CourseSectionCode = courseSectionCode.Trim().ToUpperInvariant(),
                Semester = semester,
                AcademicYear = academicYear.Trim(),
                LecturerId = lecturerId,
                MaxCapacity = maxCapacity,
                IsActive = true
            };

            courseSection.SetCreated(now);
            courseSection.SetUpdated(now);

            return courseSection;
        }

        public void UpdateInfo(string courseSectionCode, Semester semester, string academicYear, Guid lecturerId, int maxCapacity, DateTime now)
        {
            if (!IsActive)
                throw new BusinessRuleViolationException("Cannot update an inactive course section.", ErrorCodes.CourseSectionDeactivated);

            if (string.IsNullOrWhiteSpace(courseSectionCode))
                throw new BusinessRuleViolationException("Course section code cannot be empty.", ErrorCodes.InvalidCourseSectionData);

            if (string.IsNullOrWhiteSpace(academicYear))
                throw new BusinessRuleViolationException("Academic year cannot be empty.", ErrorCodes.InvalidCourseSectionData);

            if (lecturerId == Guid.Empty)
                throw new BusinessRuleViolationException("Lecturer ID cannot be empty.", ErrorCodes.InvalidCourseSectionData);

            if (maxCapacity <= 0)
                throw new BusinessRuleViolationException("Maximum capacity must be greater than zero.", ErrorCodes.InvalidCourseSectionData);

            if (maxCapacity < _enrollments.Count)
                throw new BusinessRuleViolationException("Maximum capacity cannot be less than current enrollment count.", ErrorCodes.InvalidCourseSectionData);

            CourseSectionCode = courseSectionCode.Trim().ToUpperInvariant();
            Semester = semester;
            AcademicYear = academicYear.Trim();
            LecturerId = lecturerId;
            MaxCapacity = maxCapacity;

            SetUpdated(now);
        }

        public void Deactivate(DateTime now)
        {
            if (!IsActive)
                throw new BusinessRuleViolationException("Course section is already inactive.", ErrorCodes.InvalidCourseSectionData);

            IsActive = false;
            SetUpdated(now);
        }

        public void AddEnrollment(Guid studentId, DateTime now)
        {
            if (!IsActive)
                throw new BusinessRuleViolationException("Cannot enroll into an inactive course section.", ErrorCodes.CourseSectionDeactivated);

            if (_enrollments.Count >= MaxCapacity)
                throw new BusinessRuleViolationException("Course section is full.", ErrorCodes.CourseSectionFull);

            if (studentId == Guid.Empty)
                throw new BusinessRuleViolationException("Student ID cannot be empty.", ErrorCodes.InvalidEnrollmentData);

            if (_enrollments.Any(e => e.StudentId == studentId))
                throw new BusinessRuleViolationException("Student already enrolled.", ErrorCodes.StudentAlreadyEnrolled);

            var enrollment = Enrollment.Create(Id, studentId, now);
            _enrollments.Add(enrollment);
            SetUpdated(now);
        }

        public void AddEnrollments(IEnumerable<Guid> studentIds, DateTime now)
        {
            if (!IsActive)
                throw new BusinessRuleViolationException("Cannot enroll into an inactive course section.", ErrorCodes.CourseSectionDeactivated);

            var ids = studentIds.ToList();

            // Validate empty GUIDs
            if (ids.Any(id => id == Guid.Empty))
                throw new BusinessRuleViolationException("Student ID cannot be empty.", ErrorCodes.InvalidEnrollmentData);

            // Check trùng trong chính list input (ví dụ gửi 2 lần cùng 1 ID)
            var distinctIds = ids.Distinct().ToList();
            if (distinctIds.Count != ids.Count)
                throw new BusinessRuleViolationException("Duplicate student IDs in input.", ErrorCodes.InvalidEnrollmentData);

            // Check duplicates TRƯỚC khi add bất kỳ ai
            var duplicates = distinctIds.Where(id => _enrollments.Any(e => e.StudentId == id)).ToList();

            if (duplicates.Any())
                throw new BusinessRuleViolationException(
                    $"Students already enrolled: {string.Join(", ", duplicates)}", ErrorCodes.StudentAlreadyEnrolled);

            // Check capacity TRƯỚC khi add bất kỳ ai
            if (_enrollments.Count + distinctIds.Count > MaxCapacity)
                throw new BusinessRuleViolationException("Adding these students would exceed course section capacity.", ErrorCodes.CourseSectionFull);

            // Nếu tất cả validations đều pass, thì mới add vào
            foreach (var studentId in distinctIds)
            {
                var enrollment = Enrollment.Create(Id, studentId, now);
                _enrollments.Add(enrollment);
            }
            SetUpdated(now);
        }

        public void RemoveEnrollment(Guid studentId, DateTime now)
        {
            if (!IsActive)
                throw new BusinessRuleViolationException("Cannot remove enrollment from an ended course section.", ErrorCodes.CourseSectionDeactivated);

            var enrollment = _enrollments.FirstOrDefault(e => e.StudentId == studentId)
                ?? throw new BusinessRuleViolationException("Student not enrolled.", ErrorCodes.StudentNotEnrolled);

            _enrollments.Remove(enrollment);
            SetUpdated(now);
        }

        public void AddSchedule(DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime, string room, DateTime now)
        {
            if (!IsActive)
                throw new BusinessRuleViolationException("Cannot add schedule to an ended course section.", ErrorCodes.CourseSectionDeactivated);

            if (startTime >= endTime)
                throw new BusinessRuleViolationException("Start time must be before end time.", ErrorCodes.InvalidScheduleData);

            if (string.IsNullOrWhiteSpace(room))
                throw new BusinessRuleViolationException("Room cannot be empty.", ErrorCodes.InvalidScheduleData);

            if (_schedules.Any(s =>
                s.DayOfWeek == dayOfWeek &&
                s.StartTime < endTime &&
                s.EndTime > startTime))
            {
                throw new BusinessRuleViolationException("Schedule conflicts with existing schedule.", ErrorCodes.ScheduleConflict);
            }

            var schedule = Schedule.Create(Id, dayOfWeek, startTime, endTime, room);
            _schedules.Add(schedule);
            SetUpdated(now);
        }

        public void UpdateSchedule(Guid scheduleId, DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime, string room, DateTime now)
        {
            if (!IsActive)
                throw new BusinessRuleViolationException("Cannot update schedule for an ended course section.", ErrorCodes.CourseSectionDeactivated);

            var schedule = _schedules.FirstOrDefault(s => s.Id == scheduleId)
                ?? throw new BusinessRuleViolationException("Schedule not found.", ErrorCodes.ScheduleNotFound);

            if (startTime >= endTime)
                throw new BusinessRuleViolationException("Start time must be before end time.", ErrorCodes.InvalidScheduleData);

            if (string.IsNullOrWhiteSpace(room))
                throw new BusinessRuleViolationException("Room cannot be empty.", ErrorCodes.InvalidScheduleData);

            if (_schedules.Any(s =>
                s.Id != scheduleId &&
                s.DayOfWeek == dayOfWeek &&
                s.StartTime < endTime &&
                s.EndTime > startTime))
            {
                throw new BusinessRuleViolationException("Schedule conflicts with existing schedule.", ErrorCodes.ScheduleConflict);
            }

            schedule.Update(dayOfWeek, startTime, endTime, room);
            SetUpdated(now);
        }

        public void RemoveSchedule(Guid scheduleId, DateTime now)
        {
            if (!IsActive)
                throw new BusinessRuleViolationException("Cannot remove schedule from an ended course section.", ErrorCodes.CourseSectionDeactivated);

            var schedule = _schedules.FirstOrDefault(s => s.Id == scheduleId)
                ?? throw new BusinessRuleViolationException("Schedule not found.", ErrorCodes.ScheduleNotFound);

            _schedules.Remove(schedule);
            SetUpdated(now);
        }
    }
}