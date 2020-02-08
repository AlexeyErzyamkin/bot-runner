using System;

namespace Backend.Models.Features.Jobs
{
    public class JobModel
    {
        public JobModel(Guid jobId, string name, int botsCount)
        {
            JobId = jobId;
            Name = name;
            BotsCount = botsCount;
        }

        public Guid JobId { get; set; }

        public string Name { get; set; }

        public int BotsCount { get; set; }
    }
}