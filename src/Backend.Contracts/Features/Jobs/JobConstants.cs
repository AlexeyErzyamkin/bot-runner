using System;

namespace Backend.Contracts.Features.Jobs
{
    public static class JobConstants
    {
        public static readonly Guid StreamId = new Guid("737ABF58-80B6-44B9-BE27-DB723280D3B7");

        public const string UpdatesStreamNs = "updates";
        public const string MusterStreamNs = "muster";
        public const string JobAvailableStreamNs = "job_avail";

        // public static readonly Guid JobsUpdatesStreamId = new Guid("F21FA7A5-DABB-42B8-B023-79E5A8708D07");
        // public static readonly Guid JobsMusterStreamId = new Guid("E203C982-549D-4D78-AC4D-913997C17D99");
    }
}