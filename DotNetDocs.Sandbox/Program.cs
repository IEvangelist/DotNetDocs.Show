using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetDocs.Repository;
using DotNetDocs.Services;
using DotNetDocs.Services.Extensions;
using DotNetDocs.Services.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetDocs.Sandbox
{
    class Program
    {
        static async Task Main()
        {
            try
            {
                await using (ServiceProvider services = ConfigureServices())
                {
                    IScheduleService scheduleService = services.GetService<IScheduleService>();

                    #region Add show
                    //var show = new DocsShow
                    //{
                    //    Date = GetCentralTimeZoneDateTime(new DateTime(2020, 5, 28, 11, 00, 00)).LocalTime,
                    //    Title = "",
                    //    Url = "https://www.twitch.tv/videos/634482950",
                    //    Guests = new List<Person>
                    //    {
                    //        new Person
                    //        {
                    //            FirstName = "",
                    //            LastName = "",
                    //            Email = "",
                    //            TwitterHandle = ""
                    //        }
                    //    }
                    //};

                    //_ = await scheduleService.CreateShowAsync(show);
                    //DocsShow persistedShow = await scheduleService.GetShowAsync(show.Id);
                    //if (!(persistedShow is null))
                    //{
                    //    Person guest = persistedShow.Guests.ElementAt(0);
                    //    Console.WriteLine($"Added show details for {guest.FirstName} {guest.LastName}, on {persistedShow.Date:MMM dd, yyyy}");
                    //}
                    #endregion // end add show

                    #region Read shows
                    TwitchService twitchService = services.GetService<TwitchService>();
                    IEnumerable<DocsShow> shows = await scheduleService.GetAllAsync(DateTime.Now.AddDays(-(12 * 7)));
                    foreach (DocsShow show in shows)
                    {
                        if (show.ShowImage is null && !show.IsInFuture && show.Url != "https://www.twitch.tv/thedotnetdocs")
                        {
                            var videoUri = new Uri(show.Url);
                            if (int.TryParse(videoUri.Segments.Last(), out int id))
                            {
                                var video = await twitchService.GetTwitchVideoAsync(id);
                                if (video is null) continue;

                                var url = video.thumbnails.medium.FirstOrDefault()?.url;
                                if (url is null) continue;

                                show.ShowImage = url;
                                var updatedShow = await scheduleService.UpdateShowAsync(show);
                                if (updatedShow.ShowImage == show.ShowImage)
                                {
                                    Console.WriteLine($"Updated show iamge URL!");
                                }
                            }
                        }

                        WriteShowDetails(show);
                    }
                    #endregion // end read shows
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }

        static ServiceProvider ConfigureServices()
        {
            IConfigurationBuilder configBuilder =
                new ConfigurationBuilder().AddEnvironmentVariables();
            IConfigurationRoot configuration =
                configBuilder.Build();
            IServiceCollection services =
                new ServiceCollection().AddDotNetDocsShowServices(configuration);

            return services.BuildServiceProvider();
        }

        static DateTimeWithZone GetCentralTimeZoneDateTime(DateTime date) =>
            new DateTimeWithZone(date, TimeZoneInfo.Local);

        static void WritePersonDetails(Person person, string role) =>
                    Console.WriteLine($"{role}: {person.FirstName} {person.LastName} ({person.Email}, {person.TwitterHandle})");

        static void WriteShowDetails(DocsShow show)
        {
            if (show is null)
            {
                Console.WriteLine("Show is null... sadface!");
                return;
            }

            Console.WriteLine($"{show.Title} {(show.IsInFuture ? "airs" : "aired")} on {show.Date:MMM dd, yyyy}");
            foreach (Person host in show.Hosts)
            {
                WritePersonDetails(host, "Host");
            }
            foreach (Person guest in show.Guests)
            {
                WritePersonDetails(guest, "Guest");
            }
        }
    }
}
