using DotNetDocs.Repository.Extensions;
using DotNetDocs.Services.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetDocs.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDotNetDocsShowServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHttpClient<TwitchService>();
            services.AddHttpClient<LogicAppService>();

            return services.AddCosmosDbRepository(configuration)
                           .AddSingleton<IScheduleService, ScheduleService>()
                           .Configure<TwitchOptions>(configuration.GetSection(nameof(TwitchOptions)))
                           .Configure<LogicAppOptions>(configuration.GetSection(nameof(LogicAppOptions)));
        }
    }
}
