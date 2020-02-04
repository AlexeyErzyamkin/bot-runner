using System;
using System.Threading.Tasks;

namespace Backend.Contracts.Features.Jobs
{
    using Orleans;

    public abstract class Start
    {
        public class Now : Start {}

        public class AtTime : Start
        {
            public AtTime(DateTime time)
            {
                Time = time;
            }

            public DateTime Time { get; }
        }

        public class After : Start
        {
            public After(TimeSpan time)
            {
                Time = time;
            }

            public TimeSpan Time { get; }
        }

        private Start() {}
    }

    public abstract class Stop
    {
        public class Never : Stop {}

        public class AtTime : Stop
        {
            public AtTime(DateTime time)
            {
                Time = time;
            }

            public DateTime Time { get; }
        }

        public class After : Stop
        {
            public After(TimeSpan time)
            {
                Time = time;
            }

            public TimeSpan Time { get; }
        }

        private Stop() {}
    }

    public class JobDescription
    {
        public JobDescription(Start start, Stop stop, int count)
        {
            Start = start;
            Stop = stop;
            Count = count;
        }

        public Start Start { get; }

        public Stop Stop { get; }

        public int Count { get; }
    }

    public interface IJobGrain : IGrainWithGuidKey
    {
        Task Init(JobDescription description);
    }
}