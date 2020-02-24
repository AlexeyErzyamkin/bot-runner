using System;
using System.Threading.Tasks;
using Orleans.Streams;

namespace Backend.Contracts.Streams
{
    static class StreamConsumer
    {
        public static readonly Func<Task> OnCompletedInternal = () => Task.CompletedTask;

        public static readonly Func<Exception, Task> OnErrorInternal = ex => Task.CompletedTask;
    }

    public class StreamConsumer<T> : IAsyncObserver<T>
    {
        private readonly Func<T, Task> _onNextItem;
        private readonly Func<Task> _onCompleted;
        private readonly Func<Exception, Task> _onError;

        private readonly IAsyncStream<T> _stream;

        public StreamConsumer(
            IStreamProvider streamProvider,
            Guid streamId,
            string streamNs,
            Func<T, Task> onNextItem,
            Func<Task>? onCompleted = null,
            Func<Exception, Task>? onError = null)
        {
            _onNextItem = onNextItem;
            _onCompleted = onCompleted ?? StreamConsumer.OnCompletedInternal;
            _onError = onError ?? StreamConsumer.OnErrorInternal;
            _stream = streamProvider.GetStream<T>(streamId, streamNs);
        }

        public async Task Subscribe()
        {
            await _stream.SubscribeAsync(this);
        }

        public async Task<bool> Unsubscribe()
        {
            if (await _stream.GetAllSubscriptionHandles() is {} subscriptions && subscriptions.Count > 0)
            {
                foreach (var subscription in subscriptions)
                {
                    await subscription.UnsubscribeAsync();
                }

                return true;
            }

            return false;
        }

        public async Task<bool> Resume()
        {
            if (await _stream.GetAllSubscriptionHandles() is {} subscriptions && subscriptions.Count > 0)
            {
                foreach (var subscription in subscriptions)
                {
                    await subscription.ResumeAsync(this);
                }

                return true;
            }

            return false;
        }

        Task IAsyncObserver<T>.OnNextAsync(T item, StreamSequenceToken token) => _onNextItem(item);

        Task IAsyncObserver<T>.OnCompletedAsync() => _onCompleted();

        Task IAsyncObserver<T>.OnErrorAsync(Exception ex) => _onError(ex);
    }
}