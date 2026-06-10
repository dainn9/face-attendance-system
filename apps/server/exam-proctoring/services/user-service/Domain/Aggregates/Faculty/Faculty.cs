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

        public static Faculty Create(string name, string code, DateTime now)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleViolationException("Faculty name cannot be empty.", ErrorCodes.InvalidFacultyData);

            if (string.IsNullOrWhiteSpace(code))
                throw new BusinessRuleViolationException("Faculty code cannot be empty.", ErrorCodes.InvalidFacultyData);

            var faculty = new Faculty
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
                Code = code.Trim().ToUpperInvariant()
            };
            faculty.SetCreated(now);
            faculty.SetUpdated(now);

            return faculty;
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

        public void Update(string name, string code, DateTime now)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleViolationException("Faculty name cannot be empty.", ErrorCodes.InvalidFacultyData);

            if (string.IsNullOrWhiteSpace(code))
                throw new BusinessRuleViolationException("Faculty code cannot be empty.", ErrorCodes.InvalidFacultyData);

            var normalizedName = name.Trim();
            var normalizedCode = code.Trim().ToUpperInvariant();

            if (Name == normalizedName && Code == normalizedCode)
                return;

            Name = normalizedName;
            Code = normalizedCode;
            SetUpdated(now);
        }

        public void UpdateMajor(Guid majorId, string name, string code)
        {
            var major = _majors.FirstOrDefault(m => m.Id == majorId)
                ?? throw new EntityNotFoundException("Major", majorId);

            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleViolationException("Major name cannot be empty.", ErrorCodes.InvalidMajorData);

            if (string.IsNullOrWhiteSpace(code))
                throw new BusinessRuleViolationException("Major code cannot be empty.", ErrorCodes.InvalidMajorData);

            var normalizedName = name.Trim();
            var normalizedCode = code.Trim().ToUpperInvariant();

            if (major.Name.Equals(normalizedName, StringComparison.OrdinalIgnoreCase) &&
            _majors.Any(
                m => m.Name.Equals(normalizedName, StringComparison.OrdinalIgnoreCase) &&
                m.Id != majorId))
                throw new BusinessRuleViolationException("Major name already exists.",
                    ErrorCodes.MajorNameAlreadyExists);

            if (major.Code != normalizedCode && _majors.Any(m => m.Code == normalizedCode && m.Id != majorId))
                throw new BusinessRuleViolationException("Major code already exists.", ErrorCodes.MajorCodeAlreadyExists);

            major.Update(normalizedName, normalizedCode);
        }
    }
}