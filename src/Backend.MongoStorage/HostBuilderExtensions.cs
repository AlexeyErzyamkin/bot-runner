using Backend.Features.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Backend.MongoStorage
{
    public interface IMongoStorageConfig
    {
        string Host { get; }
        ushort Port { get; }
        string User { get; }
        string Password { get; }
        string DatabaseName { get; }
    }

    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseMongoStorage(this IHostBuilder hostBuilder, IMongoStorageConfig config)
        {
            return hostBuilder.ConfigureServices(services =>
            {
                var connectionString = $"mongodb://{config.User}:{config.Password}@{config.Host}:{config.Port}";
                var storage = new MongoDbJobStorage(connectionString, config.DatabaseName);

                services.AddSingleton<IJobStorage>(storage);
            });
        }
    }
}