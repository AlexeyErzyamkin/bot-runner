namespace Backend.Contracts.Features.Workers
{
    using Orleans;
    using System.Threading.Tasks;

    public class WorkerStatus
    {
        // public int Version { get; }
        // public int UpdateVersion { get; }
        // public StartInfo StartInfo { get; }
    }

    // public abstract class StartInfo
    // {
    //     private StartInfo() {}
    //
    //     public class None : StartInfo {}
    //
    //     public class Process : StartInfo
    //     {
    //
    //     }
    // }

    public interface IWorkerGrain : IGrainWithGuidKey
    {
        ValueTask<WorkerStatus> UpdateStatus();
    }
}
