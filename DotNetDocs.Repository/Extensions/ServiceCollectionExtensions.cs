using DotNetDocs.Repository.Options;
using DotNetDocs.Repository.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetDocs.Repository.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGitHubRepository(
            this IServiceCollection services,
            IConfiguration configuration) =>
            services.AddSingleton<ICosmosContainerProvider, CosmosContainerProvider>()
                    .AddSingleton(typeof(IRepository<>), typeof(Repository<>))
                    .Configure<RepositoryOptions>(
                        configuration.GetSection(nameof(RepositoryOptions)));
    }
}