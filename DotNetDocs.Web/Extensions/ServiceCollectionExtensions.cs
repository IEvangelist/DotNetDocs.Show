using AutoMapper;
using DotNetDocs.Services.Models;
using DotNetDocs.Web.PageModels;
using Microsoft.Extensions.DependencyInjection;

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
        }
    }
}
