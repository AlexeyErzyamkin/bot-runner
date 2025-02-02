﻿using System.Threading.Tasks;
using Backend.Contracts;
// using Backend.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Hosting;

//using Backend.Features.Planet;
//using Backend.Services;

namespace Backend
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseBackend(this IHostBuilder hostBuilder)
        {
            return hostBuilder
                .UseOrleans(builder =>
                {
                    builder.UseLocalhostClustering();
                    builder.AddSimpleMessageStreamProvider(Constants.StreamProviderName);
                    builder.AddMemoryGrainStorageAsDefault();
                    builder.AddMemoryGrainStorage("PubSubStore");

                    // builder.ConfigureApplicationParts(manager =>
                    // {
                    //    manager.AddApplicationPart(typeof(IBacteriaGrain).Assembly).WithReferences();
                    // });

                    // builder.AddStartupTask(async (s, c) =>
                    // {
                    //     var client = s.GetService<IClusterClient>();
                    //     var bacteria = client.GetGrain<IBacteriaGrain>("infusoria");

                    //     await bacteria.Step();
                    // });
                    //    builder.AddGrainService<BacteriaGrainService>();
                });
        }
    }

//    class Program
//    {
//        static async Task Main(string[] args)
//        {
//            await Host.CreateDefaultBuilder(args)
////                .ConfigureLogging(builder =>
////                {
////                    builder.AddConsole();
////                })
//                .UseOrleans(builder =>
//                {
//                    builder.UseLocalhostClustering();

//                    // builder.ConfigureApplicationParts(manager =>
//                    // {
//                    //    manager.AddApplicationPart(typeof(IBacteriaGrain).Assembly).WithReferences();
//                    // });

//                    // builder.AddStartupTask(async (s, c) =>
//                    // {
//                    //     var client = s.GetService<IClusterClient>();
//                    //     var bacteria = client.GetGrain<IBacteriaGrain>("infusoria");

//                    //     await bacteria.Step();
//                    // });
//                //    builder.AddGrainService<BacteriaGrainService>();
//                })
//                .ConfigureServices(builder =>
//                {
//                    // builder.AddSingleton<IWorldStateStorage, WorldStateStorage>();
//                })
//                .RunConsoleAsync();
//        }
//    }
}
