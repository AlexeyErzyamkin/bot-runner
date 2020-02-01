using System;
using System.Threading.Tasks;

namespace Worker
{
    using Grpc.Net.Client;
    using Protos.Worker;

    class Program
    {
        private static readonly string InstanceId = Guid.NewGuid().ToString();

        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Worker.WorkerClient(channel);

            while (true)
            {
                var response = await client.UpdateStatusAsync(new StatusRequest { Id = InstanceId });

                // Console.WriteLine($"R: {response.Message}");

                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }
    }
}
