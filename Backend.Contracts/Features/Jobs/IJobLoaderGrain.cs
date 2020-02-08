using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Models.Features.Jobs;
using Orleans;

namespace Backend.Contracts.Features.Jobs
{
    public interface IJobLoaderGrain : IGrainWithIntegerKey
    {
        Task<List<JobModel>> LoadAll();

        Task<JobModel?> Load(Guid id);
    }
}