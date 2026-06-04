using api_gateway.Contracts.Attendance;
using api_gateway.Exceptions;
using BuildingBlocks.Results;

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

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<CourseSectionPagedDto>>>(cancellationToken: cancellationToken);
                return result?.Data ?? new PagedResult<CourseSectionPagedDto>();
            }
            else
                await HandleErrorAsync(response, cancellationToken);

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new DownstreamApiException((int)response.StatusCode, body);
        }
    }
}