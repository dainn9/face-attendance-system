using BuildingBlocks.Exceptions;
using SharedKernel.Core;

namespace user_service.Domain.Aggregates.Faculty
{
    public class Faculty : AggregateRoot<Guid>
    {
        public string Name { get; private set; } = null!;
        public string Code { get; private set; } = null!;

        private readonly List<Major> _majors = [];
        public IReadOnlyCollection<Major> Majors => _majors.AsReadOnly();

        private Faculty() { }

        public static Faculty Create(string name, string code)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleViolationException("Faculty name cannot be empty.", ErrorCodes.InvalidFacultyData);

            if (string.IsNullOrWhiteSpace(code))
                throw new BusinessRuleViolationException("Faculty code cannot be empty.", ErrorCodes.InvalidFacultyData);

            return new Faculty
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
                Code = code.Trim().ToUpperInvariant()
            };
        }

        public void AddMajor(string name, string code)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleViolationException("Major name cannot be empty.", ErrorCodes.InvalidMajorData);

            if (string.IsNullOrWhiteSpace(code))
                throw new BusinessRuleViolationException("Major code cannot be empty.", ErrorCodes.InvalidMajorData);

            name = name.Trim();
            code = code.ToUpperInvariant().Trim();

            if (_majors.Any(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                throw new BusinessRuleViolationException("Major name already exists.",
                    ErrorCodes.MajorNameAlreadyExists);

            if (_majors.Any(m => m.Code == code))
                throw new BusinessRuleViolationException("Major code already exists.", ErrorCodes.MajorCodeAlreadyExists);

            _majors.Add(Major.Create(Id, name, code));
        }
    }
}