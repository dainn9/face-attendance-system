namespace BuildingBlocks.Exceptions
{
    public class ForbiddenException : BaseException
    {
        public ForbiddenException(string message) : base(message, 403, ErrorCodes.Forbidden)
        { }
    }
}