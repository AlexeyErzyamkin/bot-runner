using System;
using System.Threading.Tasks;

namespace Worker
{
    using Grpc.Net.Client;
    using Protocols.Worker;

    class Program
    {
        private static readonly string InstanceId = Guid.NewGuid().ToString();

        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new WorkerProtocol.WorkerProtocolClient(channel);

            await client.RegisterAsync(new RegisterRequest());

            // while (true)
            // {
            //     var response = await client.UpdateStatusAsync(new StatusRequest { Id = InstanceId });
            //
            //     // Console.WriteLine($"R: {response.Message}");
            //
            //     await Task.Delay(TimeSpan.FromSeconds(10));
            // }
        }
    }
}
