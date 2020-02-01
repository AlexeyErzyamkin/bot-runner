// using System;
// using System.Threading.Tasks;
//
// using Orleans;
// using Orleans.Runtime;
// using Shared;
//
// namespace Backend.Features.Planet
// {
// //    public readonly struct PlanedId
// //    {
// //        private readonly long _value;
// //
// //        public PlanedId(long value)
// //        {
// //            _value = value;
// //        }
// //
// //        public bool Equals(PlanedId other)
// //        {
// //            return _value == other._value;
// //        }
// //
// //        public override bool Equals(object obj)
// //        {
// //            return obj is PlanedId other && Equals(other);
// //        }
// //
// //        public override int GetHashCode()
// //        {
// //            return _value.GetHashCode();
// //        }
// //    }
//
//     public class PlanetGrain : Grain, IPlanetGrain
//     {
// //        private readonly IPersistentState<PlanetState> _planet;
//
//         private Guid _id;
//
//         public PlanetGrain(
// //            IPersistentState<PlanetState> planet
//         )
//         {
// //            _planet = planet;
//         }
//
//         public override Task OnActivateAsync()
//         {
//             _id = this.GetPrimaryKey();
//
//             return Task.CompletedTask;
//         }
//
//         public override Task OnDeactivateAsync()
//         {
//             return Task.CompletedTask;
//         }
//     }
// }