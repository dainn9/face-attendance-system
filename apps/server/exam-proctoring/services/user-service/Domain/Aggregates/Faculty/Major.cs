using BuildingBlocks.Exceptions;

namespace user_service.Domain.Aggregates.Faculty
{
    public class Major
    {
        public Guid Id { get; private set; }
        public Guid FacultyId { get; private set; }
        public string Name { get; private set; } = null!;
        public string Code { get; private set; } = null!;

        private Major() { }

        public static Major Create(Guid facultyId, string name, string code)
        {
            if (facultyId == Guid.Empty)
                throw new BusinessRuleViolationException("Faculty ID cannot be empty.", ErrorCodes.InvalidFacultyData);

            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleViolationException("Major name cannot be empty.", ErrorCodes.InvalidMajorData);

            if (string.IsNullOrWhiteSpace(code))
                throw new BusinessRuleViolationException("Major code cannot be empty.", ErrorCodes.InvalidMajorData);

            return new Major
            {
                Id = Guid.NewGuid(),
                FacultyId = facultyId,
                Name = name.Trim(),
                Code = code.Trim().ToUpperInvariant()
            };
        }
    }
}