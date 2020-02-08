using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Features.Jobs;
using Backend.Models.Features.Jobs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Backend.MongoStorage
{
    public class MongoDbJobStorage : IJobStorage
    {
        public const string CollectionName = "jobs";

        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;

        public MongoDbJobStorage(string connectionString, string dbName)
        {
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(dbName);
        }

        public async Task<JobModel?> Load(Guid id)
        {
            var filter = Builders<MongoDbJobModel>.Filter.Eq(j => j.Model.JobId, id);

            // var jobs = _database.GetCollection<MongoDbJobModel>(CollectionName);
            var result = await Jobs
                .Find(filter)
                .FirstOrDefaultAsync();

            return result?.Model;
        }

        public async Task<List<JobModel>> LoadAll()
        {
            var filter = Builders<MongoDbJobModel>.Filter.Empty;

            // var jobs = _database.GetCollection<JobModel>(CollectionName);
            var results = await Jobs
                .Find(filter)
                .ToListAsync();

            return results
                .Select(j => j.Model)
                .ToList();
        }

        public async Task Insert(JobModel model)
        {
            // var jobs = _database.GetCollection<JobModel>(CollectionName);

            await Jobs.InsertOneAsync(new MongoDbJobModel(model));
        }

        public async Task<bool> Update(JobModel model)
        {
            // var jobs = _database.GetCollection<JobModel>(CollectionName);
            var filter = Builders<MongoDbJobModel>.Filter.Eq(j => j.Model.JobId, model.JobId);

            var replaceResult = await Jobs.ReplaceOneAsync(filter, new MongoDbJobModel(model));

            return replaceResult.IsAcknowledged && replaceResult.ModifiedCount > 0;
        }

        private IMongoCollection<MongoDbJobModel> Jobs => _database.GetCollection<MongoDbJobModel>(CollectionName);
    }

    class MongoDbJobModel
    {
        public MongoDbJobModel(JobModel model)
        {
            Model = model;
        }

        [BsonId]
        public ObjectId Id { get; set; }

        public JobModel Model { get; set; }
    }
}