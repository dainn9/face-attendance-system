using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace auth_service.Infrastructure.Caching
{
    public class RedisConnectionFactory
    {
        private readonly Lazy<ConnectionMultiplexer> _lazyConnection;
        public RedisConnectionFactory(IOptions<RedisOptions> options)
        {
            var connectionString = options.Value.ConnectionString;
            _lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(connectionString));
        }

        public IDatabase GetDatabase() => _lazyConnection.Value.GetDatabase();
    }
}