namespace SharedKernel.Core
{
    public abstract class BaseEntity<TId>
    {
        public TId Id { get; protected set; } = default!;

        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }

        // private readonly List<object> _domainEvents = new();
        // public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

        protected void SetCreated(DateTime now)
        {
            CreatedAt = now;
        }

        protected void SetUpdated(DateTime now)
        {
            UpdatedAt = now;
        }

        // protected void AddDomainEvent(object domainEvent)
        // {
        //     _domainEvents.Add(domainEvent);
        // }

        // public void ClearDomainEvents()
        // {
        //     _domainEvents.Clear();
        // }
    }
}