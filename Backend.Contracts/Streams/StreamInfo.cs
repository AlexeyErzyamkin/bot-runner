using Orleans.Streams;

namespace Backend.Contracts.Streams
{
    public static class StreamExtensions
    {
        public static string ToInfoString<T>(this IAsyncStream<T> stream)
            => $"PN = '{stream.ProviderName}', ID = '{stream.Guid.ToString()}', NS = '{stream.Namespace}'";
    }
}