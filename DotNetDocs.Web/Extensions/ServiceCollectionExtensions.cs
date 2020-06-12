using System;
using AutoMapper;
using DotNetDocs.Services.Models;
using DotNetDocs.Web.PageModels;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;

namespace DotNetDocs.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoMapper(this IServiceCollection services) =>
            services.AddSingleton(
                new MapperConfiguration(config =>
                    config.AddProfile(new MappingProfile()))
                          .CreateMapper());
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DocsShow, ShowModel>().ReverseMap();
            CreateMap<Person, PersonModel>().ReverseMap();

            // TODO:
            // Verify that this is what we want to do... and do it.
            // Right now all of our data is persisted as local time :(
            // ---------------------------------------------------------------
            // Handle converting to/from zoned datetime and datetime
            // When using datetime, we'll always persist as UTC.
            //   The cosmos-db model persists the BCL datetime using UTC.
            // When zoned datetime, we'll always convert to US/Central.
            //   The UI will allow for us to set the date, time and time zone.
            CreateMap<ZonedDateTime, DateTime>()
                .ConvertUsing<NodaDateTimeConverter>();

            CreateMap<DateTime, ZonedDateTime>()
                .ConvertUsing<NodaDateTimeConverter>();
        }
    }

    public class NodaDateTimeConverter :
        ITypeConverter<ZonedDateTime, DateTime>,
        ITypeConverter<DateTime, ZonedDateTime>
    {
        DateTime ITypeConverter<ZonedDateTime, DateTime>.Convert(
            ZonedDateTime z, DateTime _, ResolutionContext context) =>
            z.ToDateTimeUtc();

        ZonedDateTime ITypeConverter<DateTime, ZonedDateTime>.Convert(
            DateTime d, ZonedDateTime _, ResolutionContext context) =>
            new LocalDateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second)
                .InZoneLeniently(DateTimeZoneProviders.Tzdb["US/Central"]);
    }
}
