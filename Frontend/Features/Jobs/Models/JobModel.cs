using System;

namespace Frontend.Features.Jobs.Models
{
    public class JobModel
    {
        public JobModel(Guid jobId, string shortDescription, int botsCount)
        {
            JobId = jobId;
            ShortDescription = shortDescription;
            BotsCount = botsCount;
        }

        public Guid JobId { get; set; }

        public string ShortDescription { get; set; }

        public int BotsCount { get; set; }

        public int UpdateCount { get; set; }
    }
}