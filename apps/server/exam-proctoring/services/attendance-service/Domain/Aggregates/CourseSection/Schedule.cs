using BuildingBlocks.Exceptions;

namespace attendance_service.Domain.Aggregates.CourseSection
{
    public class Schedule
    {
        public Guid Id { get; private set; }
        public Guid CourseSectionId { get; private set; }
        public DayOfWeek DayOfWeek { get; private set; }
        public TimeOnly StartTime { get; private set; }
        public TimeOnly EndTime { get; private set; }
        public string Room { get; private set; } = null!;

        private Schedule() { }

        public static Schedule Create(Guid courseSectionId, DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime, string room)
        {
            if (courseSectionId == Guid.Empty)
                throw new BusinessRuleViolationException("Course section ID cannot be empty.", ErrorCodes.InvalidScheduleData);

            if (string.IsNullOrWhiteSpace(room))
                throw new BusinessRuleViolationException("Room cannot be empty.", ErrorCodes.InvalidScheduleData);

            if (endTime <= startTime)
                throw new BusinessRuleViolationException("End time must be after start time.", ErrorCodes.InvalidScheduleData);

            return new Schedule
            {
                Id = Guid.NewGuid(),
                CourseSectionId = courseSectionId,
                DayOfWeek = dayOfWeek,
                StartTime = startTime,
                EndTime = endTime,
                Room = room.Trim().ToUpperInvariant()
            };
        }

        public void Update(DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime, string room)
        {
            if (string.IsNullOrWhiteSpace(room))
                throw new BusinessRuleViolationException("Room cannot be empty.", ErrorCodes.InvalidScheduleData);

            if (endTime <= startTime)
                throw new BusinessRuleViolationException("End time must be after start time.", ErrorCodes.InvalidScheduleData);

            DayOfWeek = dayOfWeek;
            StartTime = startTime;
            EndTime = endTime;
            Room = room.Trim().ToUpperInvariant();
        }
    }
}