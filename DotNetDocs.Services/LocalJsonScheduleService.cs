using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNetDocs.Services.Models;
using Newtonsoft.Json;

namespace DotNetDocs.Services
{
    public class LocalJsonScheduleService : IScheduleService
    {
        public async ValueTask<DocsShow> CreateShowAsync(DocsShow show)
        {
            string? showFileName = $"{show.Id}.json";
            string? json = JsonConvert.SerializeObject(show);

            await File.WriteAllTextAsync(showFileName, json);

            return show;
        }

        public async ValueTask<DocsShow> DeleteShowAsync(string id)
        {
            string? showFileName = $"{id}.json";
            if (File.Exists(showFileName))
            {
                await Task.Run(() => File.Delete(showFileName));
            }

            return new DocsShow { Id = id };
        }

        public async ValueTask<IEnumerable<DocsShow>> GetAllAsync(DateTime since)
        {
            IEnumerable<(string FilePath, string FileName)>? showFiles =
                Directory.EnumerateFiles(".", "*.json", SearchOption.TopDirectoryOnly)
                         .Select(file => (FilePath: file, FileName: Path.GetFileNameWithoutExtension(file)))
                         .Where(file => Guid.TryParse(file.FileName, out Guid _));

            var shows = new List<DocsShow>();
            foreach ((string FilePath, string FileName) in showFiles)
            {
                string? json = await File.ReadAllTextAsync(FilePath);
                DocsShow? docsShow = JsonConvert.DeserializeObject<DocsShow>(json);
                if (docsShow?.Date >= since)
                {
                    shows.Add(docsShow);
                }
            }

            return shows;
        }

        public async ValueTask<DocsShow?> GetShowAsync(string id)
        {
            string? showFileName = $"{id}.json";
            if (File.Exists(showFileName))
            {
                string? json = await File.ReadAllTextAsync(showFileName);
                return JsonConvert.DeserializeObject<DocsShow>(json);
            }

            return default;
        }

        public ValueTask<DocsShow> UpdateShowAsync(DocsShow show) => CreateShowAsync(show);
    }
}
