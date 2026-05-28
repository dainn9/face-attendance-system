using BuildingBlocks.Exceptions;

namespace api_gateway.Exceptions
{
    public sealed class DownstreamApiException : BaseException
    {
        public string ResponseBody { get; }

        public DownstreamApiException(int statusCode, string responseBody) : base(
                "Downstream service error",
                statusCode,
                "DOWNSTREAM_SERVICE_ERROR")
        {
            ResponseBody = responseBody;
        }
    }
}