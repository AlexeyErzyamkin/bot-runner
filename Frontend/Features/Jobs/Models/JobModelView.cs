using Backend.Models.Features.Jobs;

namespace Frontend.Features.Jobs.Models
{
    public enum JobModelViewStatus
    {
        NotActive,
        Active,
        Deleted
    }

    public class JobModelView
    {
        public JobModelView(JobModel model)
        {
            Model = model;
        }

        public JobModel Model { get; }

        public JobModelViewStatus Status { get; set; }
    }
}