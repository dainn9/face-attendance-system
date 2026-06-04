using BuildingBlocks.Exceptions;
using SharedKernel.Core;

namespace attendance_service.Domain.Aggregates.Subject
{
    public class Subject : AggregateRoot<Guid>
    {
        public Guid? FacultyId { get; private set; }
        public string Name { get; private set; } = null!;
        public string Code { get; private set; } = null!;
        public int Credits { get; private set; }

        private Subject() { }

        public static Subject Create(Guid? facultyId, string name, string code, int credits, DateTime now)
        {
            Validate(facultyId, name, code, credits);

            var subject = new Subject
            {
                Id = Guid.NewGuid(),
                Name = name,
                Code = code.Trim().ToUpperInvariant(),
                Credits = credits,
                FacultyId = facultyId
            };

            subject.SetCreated(now);
            subject.SetUpdated(now);

            return subject;
        }

        public void Update(Guid? facultyId, string name, string code, int credits, DateTime now)
        {
            Validate(facultyId, name, code, credits);

            FacultyId = facultyId;
            Name = name;
            Code = code.Trim().ToUpperInvariant();
            Credits = credits;

            SetUpdated(now);
        }

        private static void Validate(
          Guid? facultyId,
          string name,
          string code,
          int credits)
        {
            if (facultyId.HasValue && facultyId.Value == Guid.Empty)
                throw new BusinessRuleViolationException(
                    "Faculty ID cannot be empty.",
                    ErrorCodes.InvalidSubjectData);

            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleViolationException(
                    "Name is required.",
                    ErrorCodes.InvalidSubjectData);

            if (string.IsNullOrWhiteSpace(code))
                throw new BusinessRuleViolationException(
                    "Code is required.",
                    ErrorCodes.InvalidSubjectData);

            if (credits <= 0)
                throw new BusinessRuleViolationException(
                    "Credits must be greater than 0.",
                    ErrorCodes.InvalidSubjectData);
        }
    }
}