using System;

namespace Backend.Contracts.Features.Workers
{
    public static class WorkerConstants
    {
        public static readonly Guid StreamId = new Guid("2DF954FF-22C0-4172-A8DF-B945AB0C9EBA");

        public static class StreamNs
        {
            public const string Muster = "muster";

            public const string Update = "update";
        }
    }
}