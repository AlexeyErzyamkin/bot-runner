using System.Threading.Tasks;
using Orleans;

namespace Shared
{
    public interface IPlayerGrain : IGrainWithIntegerKey
    {
        Task<PlayerInfo> GetInfo();
    }
}