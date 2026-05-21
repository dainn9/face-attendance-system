namespace auth_service.Domain.Outbox
{
    public class OutboxMessage
    {
        public Guid Id { get; private set; }
        public string Type { get; private set; } = null!;
        public string Payload { get; private set; } = null!;
        public DateTime OccurredAt { get; private set; }
        public DateTime? ProcessedAt { get; private set; }
        public int RetryCount { get; private set; }
        public DateTime? NextRetryAt { get; private set; }
        public string? Error { get; private set; }

        private OutboxMessage() { }

        public static OutboxMessage Create(string type, string payload, DateTime now)
        {
            return new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = type,
                Payload = payload,
                OccurredAt = now,
                NextRetryAt = now
            };
        }

        public void MarkProcessed(DateTime now)
        {
            ProcessedAt = now;
            Error = null;
        }

        public void MarkFailed(string error, DateTime now)
        {
            RetryCount++;
            Error = error;
            NextRetryAt = now.AddSeconds(Math.Min(300, RetryCount * 30));
        }
    }
}