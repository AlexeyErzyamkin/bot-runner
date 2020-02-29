using System;

namespace Backend.Models.Features.Workers
{
    public class WorkerModel
    {
        public WorkerModel()
        {
        }

        public WorkerModel(Guid workerId, string address)
        {
            WorkerId = workerId;
            Address = address;
        }

        public Guid WorkerId { get; set; }

        public string? Address { get; set; }
    }
}