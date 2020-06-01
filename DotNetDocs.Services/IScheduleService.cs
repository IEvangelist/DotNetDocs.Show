using DotNetDocs.Services.Models;
using System.Threading.Tasks;

namespace DotNetDocs.Services
{
    public interface IScheduleService
    {
        ValueTask<ScheduledShow> CreateShowAsync(ScheduledShow show);

        ValueTask<ScheduledShow?> GetShowAsync(string id);

        ValueTask<ScheduledShow> UpdateShowAsync(ScheduledShow show);

        ValueTask<ScheduledShow> DeleteShowAsync(string id);
    }
}