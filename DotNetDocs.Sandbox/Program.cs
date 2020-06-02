using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
                    //    Title = "eShopOnWeb with Steve Smith (@ardalis)",
                    //    Url = "https://www.twitch.tv/videos/634482950",
                    //    Guests = new List<Person>
                    //    {
                    //        new Person
                    //        {
                    //            FirstName = "Steve",
                    //            LastName = "Smith",
                    //            Email = "steve@kentsmiths.com",
                    //            TwitterHandle = "@ardalis"
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
                    IEnumerable<DocsShow> shows = await scheduleService.GetAllAsync(DateTime.Now.AddDays(-(12 * 7)));
                    foreach (DocsShow show in shows)
                    {
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

    struct DateTimeWithZone
    {
        public DateTimeWithZone(DateTime dateTime, TimeZoneInfo timeZone)
        {
            var dateTimeUnspec = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
            UniversalTime = TimeZoneInfo.ConvertTimeToUtc(dateTimeUnspec, timeZone);
            TimeZone = timeZone;
        }

        public DateTime UniversalTime { get; }

        public TimeZoneInfo TimeZone { get; }

        public DateTime LocalTime => TimeZoneInfo.ConvertTime(UniversalTime, TimeZone);
    }
}
