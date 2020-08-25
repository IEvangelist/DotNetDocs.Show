using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DotNetDocs.Services;
using DotNetDocs.Services.Models;
using DotNetDocs.Web.Extensions;
using DotNetDocs.Web.Workers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace DotNetDocs.Web.Controllers
{
    public class ReallySimpleSyndicationController : Controller
    {
        readonly IMemoryCache? _cache;
        readonly IScheduleService? _scheduleService;

        public ReallySimpleSyndicationController(
            IMemoryCache cache, IScheduleService scheduleService) =>
            (_cache, _scheduleService) = (cache, scheduleService);

        [AllowAnonymous, HttpGet("rss"), ResponseCache(Duration = 1200)]
        public async Task<IActionResult> Rss()
        {
            var host = $"{Request.Scheme}://{Request.Host.ToUriComponent()}";
            Uri siteUri = new Uri(host);
            var feed = new SyndicationFeed(
                "The .NET Docs Show",
                "The RSS feed containing past, present and future episodes of The .NET Docs Show. You're weekly dose of the .NET #DeveloperCommunity.",
                siteUri,
                $"{host}/rss",
                DateTime.UtcNow)
            {
                Copyright = new TextSyndicationContent($"© {DateTime.UtcNow.Year} The .NET Docs Show")
            };

            IEnumerable<DocsShow> shows = await _cache.GetOrCreateAsync(
                CacheKeys.ShowSchedule,
                async _ =>
                await _scheduleService!.GetAllAsync(new DateTime(2020, 1, 1)));

            feed.Items = shows.Where(show => show.IsScheduled && show.IsPublished)
                     .OrderByDescending(show => show.Date)
                     .Select(show =>
                        new SyndicationItem(
                            show.Title,
                            show.ToRssDescription(),
                            new Uri($"{host}/show/{show.Id}"),
                            show.Id,
                            show.Date!.Value))
                     .ToList();

            using var stream = new MemoryStream();
            using var xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings
            {
                Async = true,
                Encoding = Encoding.UTF8,
                NewLineHandling = NewLineHandling.Entitize,
                NewLineOnAttributes = true,
                Indent = true
            });

            var rssFormatter = new Rss20FeedFormatter(feed, false);
            rssFormatter.WriteTo(xmlWriter);
            await xmlWriter.FlushAsync();

            return File(stream.ToArray(), "application/rss+xml; charset=utf-8");
        }
    }
}
