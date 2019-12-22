using System;
using System.Threading.Tasks;
using Backend.Services;
using Orleans;
using Shared;

namespace Backend.Features.World
{
    [Serializable]
    public class WorldState
    {
        public Cosmos.Domain.World.T? World { get; set; }
    }

    public class WorldGrain : Grain, IWorldGrain
    {
        private readonly IWorldStateStorage _worldStateStorage;

        private string? _id;
        private WorldState? _worldState;

        public WorldGrain(IWorldStateStorage worldStateStorage)
        {
            _worldStateStorage = worldStateStorage;
        }

        public override async Task OnActivateAsync()
        {
            _id = this.GetPrimaryKeyString();

            _worldState = await _worldStateStorage.Get(_id) ?? new WorldState();
        }

        public override async Task OnDeactivateAsync()
        {
            if (_worldState != null)
            {
                await _worldStateStorage.Set(_id!, _worldState);
            }
        }

        public Task Initialize(Cosmos.Domain.World.T world)
        {
            _worldState!.World = world;

            return Task.CompletedTask;
        }
    }
}