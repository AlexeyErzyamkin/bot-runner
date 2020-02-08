using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Models.Features.Jobs;

namespace Backend.Features.Jobs
{
    public interface IJobStorage
    {
        Task<JobModel?> Load(Guid id);

        Task<List<JobModel>> LoadAll();

        Task Insert(JobModel model);

        Task<bool> Update(JobModel model);
    }
}