using System;
using System.Threading.Tasks;
using Orleans.Streams;

namespace Backend.Contracts.Streams
{
    public class StreamProducer<T>
    {
        private readonly IAsyncStream<T> _stream;

        public StreamProducer(
            IStreamProvider streamProvider,
            Guid streamId,
            string streamNs)
        {
            _stream = streamProvider.GetStream<T>(streamId, streamNs);
        }

        public async Task Next(T item)
        {
            await _stream.OnNextAsync(item);
        }
    }
}