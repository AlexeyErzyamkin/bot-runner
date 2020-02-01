// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Orleans;
// using Orleans.Core;
// using Orleans.Runtime;
// using Shared;
//
// namespace Backend.Features.Player
// {
//     public class PlayerGrain : Grain, IPlayerGrain
//     {
//         // private readonly long _id = 1;
//
//         private readonly List<Guid> _planets = new List<Guid>();
//
// //        private readonly IPersistentState<PlayerState> _state;
// //        private readonly IPersistentState<PlayerItems> _items;
//
//         public PlayerGrain(
// //            IPersistentState<PlayerState> state,
// //            IPersistentState<PlayerItems> items
//         )
//         {
// //            _state = state;
// //            _items = items;
//         }
//
//         public Task<PlayerInfo> GetInfo()
//         {
//             // var world = Cosmos.Domain.World.create(
//             //     Cosmos.Domain.World.Width.NewWidth(100),
//             //     Cosmos.Domain.World.Height.NewHeight(100),
//             //     100
//             // );
//             //
//             // foreach (var eachPlanet in world.Planets)
//             // {
//             //     Console.WriteLine($"Planet X:{eachPlanet.X.Item}; Y:{eachPlanet.Y.Item}");
//             // }
//
//             var info = new PlayerInfo(
//                 "Guest1"
//             );
//
//             return Task.FromResult(info);
//         }
//     }
// }