using System;
using System.Threading.Tasks;
using Backend.Contracts;
using Frontend.Features.Jobs.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;

namespace Frontend
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var clusterClient = new ClientBuilder()
                .UseLocalhostClustering()
                .AddSimpleMessageStreamProvider(Constants.StreamProviderName)
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
                .ConfigureServices(services =>
                {
                    services.AddSingleton(clusterClient);
                    services.AddSingleton<IJobService, JobService>();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //
                    webBuilder.UseStartup<Startup>();
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
