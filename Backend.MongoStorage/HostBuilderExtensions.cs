using Backend.Features.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Backend.MongoStorage
{
    public interface IMongoStorageConfig
    {
        string ConnectionString { get; }
        string DatabaseName { get; }
    }

    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseMongoStorage(this IHostBuilder hostBuilder, IMongoStorageConfig config)
        {
            return hostBuilder.ConfigureServices(services =>
            {
                var storage = new MongoDbJobStorage(config.ConnectionString, config.DatabaseName);

                services.AddSingleton<IJobStorage>(storage);
            });
        }
    }
}