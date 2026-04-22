namespace BuildingBlocks.Exceptions
{
    public class BusinessRuleViolationException : BaseException
    {
        public BusinessRuleViolationException(string message, string errorCode) : base(message, 400, errorCode)
        { }
    }
}