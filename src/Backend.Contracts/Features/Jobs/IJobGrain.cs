using System;
using System.Threading.Tasks;
using Backend.Models.Features.Jobs;

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
        public JobDescription(string scenarioName, int botsCount, int botsCountAtSameTime, int botStartDelay)
        {
            ScenarioName = scenarioName;
            BotsCount = botsCount;
            BotsCountAtSameTime = botsCountAtSameTime;
            BotStartDelay = botStartDelay;
        }

        // public Start Start { get; }
        //
        // public Stop Stop { get; }

        public string ScenarioName { get; }

        public int BotsCount { get; }

        public int BotsCountAtSameTime { get; }

        public int BotStartDelay { get; }
    }

    public interface IJobGrain : IGrainWithGuidKey
    {
        Task Update(JobModel model);

        Task Start();

        Task Stop();
    }
}