using DotNetDocs.Services.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetDocs.Services
{
    public interface IScheduleService
    {
        ValueTask<DocsShow> CreateShowAsync(DocsShow show);

        ValueTask<IEnumerable<DocsShow>> GetAllAsync(DateTime since);

        ValueTask<DocsShow> GetShowAsync(string id);

        ValueTask<DocsShow> UpdateShowAsync(DocsShow show);

        ValueTask<DocsShow> DeleteShowAsync(string id);
    }
}
