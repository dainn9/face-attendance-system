namespace BuildingBlocks.Exceptions
{
    public class ConfigurationException : BaseException
    {
        public ConfigurationException(string message) : base(message, 500, ErrorCodes.ConfigurationError)
        { }
    }
}