namespace user_service.Domain.Aggregates.User
{
    public class LecturerProfile
    {
        public Guid UserId { get; private set; }
        public string LecturerCode { get; private set; } = null!;
        public string FacultyCode { get; private set; } = null!;

        private LecturerProfile() { }

        public static LecturerProfile Create(Guid userId, string lecturerCode, string facultyCode)
        {
            return new LecturerProfile
            {
                UserId = userId,
                LecturerCode = lecturerCode,
                FacultyCode = facultyCode
            };
        }
    }
}