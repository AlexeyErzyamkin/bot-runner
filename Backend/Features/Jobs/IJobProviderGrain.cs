using System.Threading.Tasks;
using Orleans;

namespace Backend.Features.Jobs
{
    public interface IJobProviderGrain : IGrainWithGuidKey
    {
        Task RequestJob();
    }
}