using System;

namespace Backend.Models.Features.Jobs
{
    public class JobModel
    {
        public Guid JobId { get; set; }

        public string? Name { get; set; }

        public string? AuthServer { set; get; }

        public string? Scenario { get; set; }

        public string? ScenarioParams { get; set; }

        public int BotsCount { get; set; }

        public int MaxDegreeOfParallelism { get; set; }

        public int BotStartDelay { get; set; }
    }
}