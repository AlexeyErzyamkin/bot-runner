// using System.Threading.Tasks;
// using Cosmos.Domain;
// using Orleans;
// using Shared;
//
// namespace Frontend.Services
// {
//     public class WorldsService
//     {
//         private readonly IClusterClient _clusterClient;
//
//         public WorldsService(IClusterClient clusterClient)
//         {
//             _clusterClient = clusterClient;
//         }
//
//         public async Task Create(string worldId, uint width, uint height, int planetsCount)
//         {
//             var universe = World.create(
//                 World.Width.NewWidth(width),
//                 World.Height.NewHeight(height),
//                 planetsCount
//             );
//
//             var worldGrain = _clusterClient.GetGrain<IWorldGrain>(worldId);
//             await worldGrain.Initialize(new World.T(World.WorldId.NewWorldId(worldId), universe));
//         }
//     }
// }