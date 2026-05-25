using BuildingBlocks.Exceptions;
using SharedKernel.Core;

namespace attendance_service.Domain.Aggregates.Course
{
    public class Course : AggregateRoot<Guid>
    {
        public string Name { get; private set; } = null!;
        public string Code { get; private set; } = null!;
        public int Credits { get; private set; }

        private Course() { }

        public static Course Create(string name, string code, int credits, DateTime now)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleViolationException("Name is required.", ErrorCodes.CourseNameRequired);

            if (string.IsNullOrWhiteSpace(code))
                throw new BusinessRuleViolationException("Code is required.", ErrorCodes.CourseCodeRequired);

            if (credits <= 0)
                throw new BusinessRuleViolationException("Credits must be greater than 0.", ErrorCodes.CourseCreditsInvalid);

            var course = new Course
            {
                Id = Guid.NewGuid(),
                Name = name,
                Code = code,
                Credits = credits,
            };

            course.SetCreated(now);
            course.SetUpdated(now);

            return course;
        }

        public void Update(string name, string code, int credits, DateTime now)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleViolationException("Name is required.", ErrorCodes.CourseNameRequired);

            if (string.IsNullOrWhiteSpace(code))
                throw new BusinessRuleViolationException("Code is required.", ErrorCodes.CourseCodeRequired);

            if (credits <= 0)
                throw new BusinessRuleViolationException("Credits must be greater than 0.", ErrorCodes.CourseCreditsInvalid);

            Name = name;
            Code = code;
            Credits = credits;

            SetUpdated(now);
        }
    }
}