namespace DapperExample.Web.Exceptions;

public sealed class EntityNotFoundException : Exception
{
    public EntityNotFoundException(Guid entityId)
        : base($"The entity with id '{entityId}' doesn't exist in the database.")
    {
    }
}