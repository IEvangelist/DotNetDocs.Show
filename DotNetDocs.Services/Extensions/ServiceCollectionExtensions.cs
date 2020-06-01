using DotNetDocs.Repository.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetDocs.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDotNetDocsShowServices(
            this IServiceCollection services,
            IConfiguration configuration) =>
            services.AddCosmosDbRepository(configuration)
                    .AddSingleton<IScheduleService, ScheduleService>();
    }
}
