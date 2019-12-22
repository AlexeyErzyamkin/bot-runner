using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;

namespace Frontend
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var clusterClient = new ClientBuilder()
                .UseLocalhostClustering()
                .Build();

            var retryFilter = CreateRetryFilter();
            await clusterClient.Connect(retryFilter);

            await CreateHostBuilder(args, clusterClient)
                .Build()
                .RunAsync();

            await clusterClient.Close();
        }

        private static IHostBuilder CreateHostBuilder(string[] args, IClusterClient clusterClient)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureServices(services =>
                {
                    //
                    services.AddSingleton(clusterClient);
                });
        }

        private static Func<Exception, Task<bool>> CreateRetryFilter()
        {
            var attempt = 0;
            return RetryFilter;

            async Task<bool> RetryFilter(Exception ex)
            {
                attempt += 1;

                Console.WriteLine("Error connecting to the Orleans cluster. Attempt: {0}", attempt);

                if (attempt > 5)
                {
                    Console.WriteLine("Finished trying to connect to the Orleans cluster");

                    return false;
                }

                await Task.Delay(TimeSpan.FromSeconds(2));

                return true;
            }
        }
    }
}
