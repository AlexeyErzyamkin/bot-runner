// using System.Threading.Tasks;
// using Orleans;
// using Shared;
//
// namespace Frontend.Services
// {
//     public class PlayersService
//     {
//         private readonly IClusterClient _clusterClient;
//
//         public PlayersService(IClusterClient clusterClient)
//         {
//             _clusterClient = clusterClient;
//         }
//
//         public async Task<PlayerInfo> GetInfo(long playerId)
//         {
//             var playerGrain = _clusterClient.GetGrain<IPlayerGrain>(playerId);
//
//             return await playerGrain.GetInfo();
//         }
//     }
// }