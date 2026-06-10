namespace BuildingBlocks.Results
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? ErrorCode { get; set; }
        public string? TraceId { get; set; }
        public object? Errors { get; set; }
        public T? Data { get; set; }
    }
}