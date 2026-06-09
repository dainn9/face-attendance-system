using api_gateway.Contracts.Attendance;
using api_gateway.Contracts.Users;
using BuildingBlocks.Results;
using SharedKernel.Core.Enums;

namespace api_gateway.Clients
{
    public class AttendanceClient : BaseHttpClient
    {
        public AttendanceClient(HttpClient httpClient, ILogger<AttendanceClient> logger)
        : base(httpClient, logger) { }

        public async Task<PagedResult<CourseSectionPagedDto>> GetCourseSectionPagedAsync(GetCourseSectionPagedRequest request, CancellationToken cancellationToken = default)
        {
            var queryString = QueryString.Create(new Dictionary<string, string?>
            {
                ["page"] = request.Page.ToString(),
                ["pageSize"] = request.PageSize.ToString(),
                ["searchQuery"] = request.SearchQuery,
                ["facultyId"] = request.FacultyId?.ToString(),
                ["semester"] = request.Semester,
                ["academicYear"] = request.AcademicYear,
                ["isActive"] = request.IsActive?.ToString()
            });

            var response = await _httpClient.GetAsync($"api/v1/internal/course-sections{queryString}", cancellationToken);

            if (!response.IsSuccessStatusCode)
                await HandleErrorAsync(response, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<CourseSectionPagedDto>>>(cancellationToken: cancellationToken);
            return result?.Data ?? new PagedResult<CourseSectionPagedDto>();
        }

        public async Task<Guid> CreateCourseSectionAsync(CreateCourseSectionRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("api/v1/internal/course-sections", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
                await HandleErrorAsync(response, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>(cancellationToken: cancellationToken);
            return result?.Data ?? throw new InvalidOperationException("Attendance service returned empty response.");
        }

        public async Task<CourseSectionDetailDto> GetCourseSectionDetailAsync(
            Guid courseSectionId,
            Guid userId,
            UserRole role,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"api/v1/internal/course-sections/{courseSectionId}" +
            $"?userId={userId}&role={role}"
            , cancellationToken);

            if (!response.IsSuccessStatusCode)
                await HandleErrorAsync(response, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<CourseSectionDetailDto>>(cancellationToken: cancellationToken);
            return result?.Data ?? throw new InvalidOperationException("Attendance service returned empty response.");
        }

        public async Task<PagedResult<Guid>> GetEnrolledStudentIdsPagedAsync(
            Guid courseSectionId,
            int page,
            int pageSize,
            CancellationToken cancellationToken
        )
        {
            var queryString = QueryString.Create(new Dictionary<string, string?>
            {
                ["page"] = page.ToString(),
                ["pageSize"] = pageSize.ToString()
            });

            var response = await _httpClient.GetAsync($"api/v1/internal/course-sections/{courseSectionId}/students{queryString}", cancellationToken);

            if (!response.IsSuccessStatusCode)
                await HandleErrorAsync(response, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<Guid>>>(cancellationToken: cancellationToken);
            return result?.Data ?? new PagedResult<Guid>();
        }

        public async Task EnrollStudentsAsync(Guid courseSectionId, IReadOnlyList<Guid> studentIds, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/v1/internal/course-sections/{courseSectionId}/enrollments", studentIds, cancellationToken);

            if (!response.IsSuccessStatusCode)
                await HandleErrorAsync(response, cancellationToken);

            return;
        }

        public async Task<Dictionary<Guid, StudentAttendanceSummaryDto>> GetStudentAttendanceSummariesAsync(
            Guid courseSectionId,
            IReadOnlyList<Guid> studentIds,
            CancellationToken cancellationToken = default
        )
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"api/v1/internal/course-sections/{courseSectionId}/students/attendance-summaries",
                studentIds,
                cancellationToken
            );

            if (!response.IsSuccessStatusCode)
                await HandleErrorAsync(response, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<Dictionary<Guid, StudentAttendanceSummaryDto>>>(cancellationToken: cancellationToken);
            return result?.Data ?? new Dictionary<Guid, StudentAttendanceSummaryDto>();
        }

        public async Task<Dictionary<Guid, AttendanceRecordDto>> GetAttendanceRecordsAsync(
            Guid attendanceSessionId,
            CancellationToken cancellationToken = default
        )
        {
            var response = await _httpClient.GetAsync($"api/v1/internal/attendance-sessions/{attendanceSessionId}/records", cancellationToken);

            if (!response.IsSuccessStatusCode)
                await HandleErrorAsync(response, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<Dictionary<Guid, AttendanceRecordDto>>>(cancellationToken: cancellationToken);
            return result?.Data ?? new Dictionary<Guid, AttendanceRecordDto>();
        }
    }
}