using DotNetDocs.Repository;
using DotNetDocs.Services.Models;
using System.Threading.Tasks;

namespace DotNetDocs.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IRepository<ScheduledShow> _showRepository;

        public ScheduleService(IRepository<ScheduledShow> showRepository) => _showRepository = showRepository;

        public ValueTask<ScheduledShow> CreateShowAsync(ScheduledShow show) => _showRepository.CreateAsync(show);

        public ValueTask<ScheduledShow> DeleteShowAsync(string id) => _showRepository.DeleteAsync(id);

        public ValueTask<ScheduledShow?> GetShowAsync(string id) => _showRepository.GetAsync(id);

        public ValueTask<ScheduledShow> UpdateShowAsync(ScheduledShow show) => _showRepository.UpdateAsync(show);
    }
}
