// using System.Collections.Concurrent;
// using System.Threading.Tasks;
// using Backend.Features.World;
//
// namespace Backend.Services
// {
//     public interface IWorldStateStorage
//     {
//         Task<WorldState?> Get(string worldId);
//
//         Task Set(string worldId, WorldState worldState);
//     }
//
//     public class WorldStateStorage : IWorldStateStorage
//     {
//         private readonly ConcurrentDictionary<string, WorldState> _worldStates = new ConcurrentDictionary<string, WorldState>();
//
//         public Task<WorldState?> Get(string worldId)
//         {
//             _worldStates.TryGetValue(worldId, out var result);
//
//             return Task.FromResult(result);
//         }
//
//         public Task Set(string worldId, WorldState worldState)
//         {
//             _worldStates.GetOrAdd(worldId, (key, s) => s, worldState);
//
//             return Task.CompletedTask;
//         }
//     }
// }