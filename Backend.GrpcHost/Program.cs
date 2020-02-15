using Backend.MongoStorage;

namespace Backend.GrpcHost
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Hosting;
    using Backend;
    using System.Threading.Tasks;
    using Backend.Features.Jobs;

    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args)
                .Build()
                .RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var config = new FakeMongoStorageConfig();

            return Host.CreateDefaultBuilder(args)
                .UseBackend()
                //.UseMongoStorage(config)
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IJobStorage>(new FakeJobStorage());
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //
                    webBuilder.UseStartup<Startup>();
                });
        }
    }

    class FakeMongoStorageConfig : IMongoStorageConfig
    {
        public string Host => "localhost";

        public ushort Port => 27017;

        public string User => "root";

        public string Password => "12345";

        public string DatabaseName => "backend";
    }
}
