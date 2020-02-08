using Backend.MongoStorage;

namespace Backend.GrpcHost
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.AspNetCore.Hosting;
    using Backend;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var config = new FakeMongoStorageConfig();

            return Host.CreateDefaultBuilder(args)
                .UseBackend()
                .UseMongoStorage(config)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }

    class FakeMongoStorageConfig : IMongoStorageConfig
    {
        public string ConnectionString => "mongodb://root:12345@localhost:27017";
        public string DatabaseName => "backend";
    }
}
