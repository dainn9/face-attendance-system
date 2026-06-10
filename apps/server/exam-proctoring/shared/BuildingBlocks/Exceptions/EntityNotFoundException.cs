namespace BuildingBlocks.Exceptions
{
    public class EntityNotFoundException : BaseException
    {
        public EntityNotFoundException(string message)
            : base(message, 404, ErrorCodes.EntityNotFound) { }

        public EntityNotFoundException(string entity, object key)
            : base($"{entity} with id '{key}' not found", 404, ErrorCodes.EntityNotFound) { }
    }
}