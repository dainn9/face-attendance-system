using auth_service.Application.Abstractions.System;

namespace auth_service.Infrastructure.System
{
    public class SystemClock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}