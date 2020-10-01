using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetDocs.Services.Models;
using Microsoft.Azure.CosmosRepository;

namespace DotNetDocs.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IRepository<DocsShow> _showRepository;

        public ScheduleService(IRepository<DocsShow> showRepository) =>
            _showRepository = showRepository;

        public ValueTask<DocsShow> CreateShowAsync(DocsShow show) =>
            _showRepository.CreateAsync(show);

        public ValueTask DeleteShowAsync(string id) =>
            _showRepository.DeleteAsync(id);

        public ValueTask<DocsShow> GetShowAsync(string id) =>
            _showRepository.GetAsync(id);

        public ValueTask<IEnumerable<DocsShow>> GetAllAsync(DateTime since) =>
            _showRepository.GetAsync(show => show.Date >= since);

        public ValueTask<DocsShow> UpdateShowAsync(DocsShow show) =>
            _showRepository.UpdateAsync(show);
    }
}
