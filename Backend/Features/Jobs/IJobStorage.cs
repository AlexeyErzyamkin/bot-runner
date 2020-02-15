using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Models.Features.Jobs;
using Shared;

namespace Backend.Features.Jobs
{
    public interface IJobStorage
    {
        Task<JobModel?> Load(Guid id);

        Task<List<JobModel>> LoadAll();

        Task Insert(JobModel model);

        Task<bool> Update(JobModel model);
    }

    public class FakeJobStorage : IJobStorage
    {
        private readonly ConcurrentDictionary<Guid, JobModel> _jobs = new ConcurrentDictionary<Guid, JobModel>();

        public FakeJobStorage()
        {
        }

        public FakeJobStorage(JobModel[] models)
        {
            foreach (var eachModel in models)
            {
                _jobs[eachModel.JobId] = eachModel;
            }
        }

        public Task Insert(JobModel model)
        {
            _ = _jobs.TryAdd(model.JobId, model).Ensure();

            return Task.CompletedTask;
        }

        public Task<JobModel?> Load(Guid id)
        {
            _jobs.TryGetValue(id, out var model);

            return Task.FromResult(model);
        }

        public Task<List<JobModel>> LoadAll()
        {
            var models = _jobs.Values.ToList();

            return Task.FromResult(models);
        }

        public Task<bool> Update(JobModel model)
        {
            _jobs[model.JobId] = model;

            return Task.FromResult(true);
        }
    }
}