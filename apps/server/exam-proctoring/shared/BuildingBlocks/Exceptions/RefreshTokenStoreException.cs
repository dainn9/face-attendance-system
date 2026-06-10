namespace BuildingBlocks.Exceptions
{
    public class RefreshTokenStoreException : BaseException
    {
        public RefreshTokenStoreException(string message) : base(message, 500, ErrorCodes.RefreshTokenStoreFailed)
        { }
    }
}