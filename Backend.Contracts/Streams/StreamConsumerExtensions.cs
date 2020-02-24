using System.Threading.Tasks;

namespace Backend.Contracts.Streams
{
    public static class StreamConsumerExtensions
    {
        public static async Task ResumeOrSubscribe<T>(this StreamConsumer<T> stream)
        {
            var resumed = await stream.Resume();

            if (!resumed)
            {
                await stream.Subscribe();
            }
        }
    }
}