using System.Threading.Tasks;
using Frontend.Features.Jobs.Models;

namespace Frontend.Features.Jobs
{
    public delegate Task JobFunc(JobModelView job);
}