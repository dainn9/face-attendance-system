namespace BuildingBlocks.Exceptions
{
    public class ErrorCodes
    {
        // ======= GENERAL =======
        public const string InternalServerError = "INTERNAL_SERVER_ERROR";
        public const string Token_Expired = "TOKEN_EXPIRED";
        public const string NotFound = "NOT_FOUND";
        public const string EntityNotFound = "ENTITY_NOT_FOUND";
        public const string Unauthorized = "UNAUTHORIZED";
        public const string InvalidSessionType = "INVALID_SESSION_TYPE";
        public const string InvalidRefreshToken = "INVALID_REFRESH_TOKEN";
        public const string ConfigurationError = "CONFIGURATION_ERROR";

        // ======= ACCOUNT =======
        public const string AccountNotFound = "ACCOUNT_NOT_FOUND";
        public const string AccountNotActive = "ACCOUNT_NOT_ACTIVE";
        public const string AccountLocked = "ACCOUNT_LOCKED";

        // ======= EMAIL =======
        public const string EmailEmpty = "EMAIL_EMPTY";
        public const string InvalidEmail = "INVALID_EMAIL";
        public const string EmailAlreadyExists = "EMAIL_ALREADY_EXISTS";

        // ======= PASSWORD =======
        public const string PasswordEmpty = "PASSWORD_EMPTY";
        public const string PasswordIncorrect = "PASSWORD_INCORRECT";

        // ======= REFRESH TOKEN =======
        public const string RefreshTokenStoreFailed = "REFRESH_TOKEN_STORE_FAILED";

        // ======= USER SERVICE =======
        public const string LecturerProfileAlreadyExists = "LECTURER_PROFILE_ALREADY_EXISTS";
        public const string StudentProfileAlreadyExists = "STUDENT_PROFILE_ALREADY_EXISTS";
        public const string InvalidUserRole = "INVALID_USER_ROLE";
        public const string InvalidUserData = "INVALID_USER_DATA";
        public const string InvalidLecturerProfile = "INVALID_LECTURER_PROFILE";
        public const string InvalidStudentProfile = "INVALID_STUDENT_PROFILE";
        public const string UserCodeAlreadyExists = "USER_CODE_ALREADY_EXISTS";
        // ======= FACULTY =======
        public const string FacultyAlreadyExists = "FACULTY_ALREADY_EXISTS";
        public const string MajorAlreadyExists = "MAJOR_ALREADY_EXISTS";
        public const string MajorNameAlreadyExists = "MAJOR_NAME_ALREADY_EXISTS";
        public const string MajorCodeAlreadyExists = "MAJOR_CODE_ALREADY_EXISTS";
        public const string InvalidFacultyData = "INVALID_FACULTY_DATA";
        public const string InvalidMajorData = "INVALID_MAJOR_DATA";
        public const string FacultyNameAlreadyExists = "FACULTY_NAME_ALREADY_EXISTS";
        public const string FacultyCodeAlreadyExists = "FACULTY_Code_ALREADY_EXISTS";

        // ======= FACULTY =======
        public const string FacultyAlreadyExists = "FACULTY_ALREADY_EXISTS";
        public const string MajorAlreadyExists = "MAJOR_ALREADY_EXISTS";
        public const string MajorNameAlreadyExists = "MAJOR_NAME_ALREADY_EXISTS";
        public const string MajorCodeAlreadyExists = "MAJOR_CODE_ALREADY_EXISTS";
        public const string InvalidFacultyData = "INVALID_FACULTY_DATA";
        public const string InvalidMajorData = "INVALID_MAJOR_DATA";

        // ======= ATTENDANCE SERVICE =======
        public const string StudentAlreadyCheckedIn = "STUDENT_ALREADY_CHECKED_IN";
        public const string StudentAlreadyEnrolled = "STUDENT_ALREADY_ENROLLED";
        public const string StudentNotEnrolled = "STUDENT_NOT_ENROLLED";
        public const string AttendanceSessionNotOpen = "ATTENDANCE_SESSION_NOT_OPEN";

        // ======= COURSE =======
        public const string CourseCodeAlreadyExists = "COURSE_CODE_ALREADY_EXISTS";
        public const string InvalidCourseData = "INVALID_COURSE_DATA";

        // ======= COURSE SECTION =======
        public const string InvalidCourseSectionData = "INVALID_COURSE_SECTION_DATA";

        // ======= ENROLLMENT =======
        public const string InvalidEnrollmentData = "INVALID_ENROLLMENT_DATA";
    }
}