using System;

namespace Backend.Models.Features.Jobs
{
    public class JobModel
    {
        public JobModel()
        {
        }

        public JobModel(Guid jobId, string? name, string? authServer, string? scenario, string? scenarioParams, int botsCount, int maxDegreeOfParallelism, int botStartDelay)
        {
            JobId = jobId;
            Name = name;
            AuthServer = authServer;
            Scenario = scenario;
            ScenarioParams = scenarioParams;
            BotsCount = botsCount;
            MaxDegreeOfParallelism = maxDegreeOfParallelism;
            BotStartDelay = botStartDelay;
        }

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