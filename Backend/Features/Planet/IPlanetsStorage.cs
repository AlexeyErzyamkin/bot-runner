// using System;
// using System.Threading.Tasks;
//
// namespace Backend.Features.Planet
// {
//     public interface IPlanetsStorage
//     {
//         Task<PlanetState?> LoadState(Guid id);
//
//         Task<PlanetState> CreateState(Guid id);
//
//         Task SaveState(Guid id, PlanetState state);
//     }
// }