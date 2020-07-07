using System;
using AutoMapper;
using DotNetDocs.Services.Models;
using DotNetDocs.Web.Options;
using DotNetDocs.Web.PageModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;

namespace DotNetDocs.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoMapper(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDataProtection();
            return services.Configure<FeatureOptions>(
                configuration.GetSection(nameof(FeatureOptions)))
                .AddSingleton(
                    new MapperConfiguration(config =>
                        config.AddProfile(new MappingProfile()))
                              .CreateMapper());
        }
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DocsShow, ShowModel>().ReverseMap();
            CreateMap<Person, PersonModel>().ReverseMap();

            CreateMap<ZonedDateTime, DateTimeOffset>().ConvertUsing<NodaDateTimeConverter>();
            CreateMap<DateTimeOffset, ZonedDateTime>().ConvertUsing<NodaDateTimeConverter>();
        }
    }

    public class NodaDateTimeConverter :
        ITypeConverter<ZonedDateTime, DateTimeOffset>,
        ITypeConverter<DateTimeOffset, ZonedDateTime>
    {
        DateTimeOffset ITypeConverter<ZonedDateTime, DateTimeOffset>.Convert(
            ZonedDateTime z, DateTimeOffset _, ResolutionContext context) =>
            z.ToDateTimeOffset();

        ZonedDateTime ITypeConverter<DateTimeOffset, ZonedDateTime>.Convert(
            DateTimeOffset d, ZonedDateTime _, ResolutionContext context) =>
            ZonedDateTime.FromDateTimeOffset(d);
    }
}
