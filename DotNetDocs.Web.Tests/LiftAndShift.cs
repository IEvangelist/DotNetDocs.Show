using System;
using System.Threading.Tasks;
using DotNetDocs.Services.Extensions;
using DotNetDocs.Services.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace DotNetDocs.Web.Tests
{
    public class LiftAndShift
    {
        [Fact]
        public async Task ReadExistingDataAndSaveItToNewDatabase()
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .ConfigureKeyVault()
                .Build();

            using var serviceProvider = new ServiceCollection()
                .AddSingleton<IConfiguration>(configuration)
                .AddDotNetDocsShowServices(configuration)
                .BuildServiceProvider();

            var repo = serviceProvider.GetRequiredService<IRepository<DocsShow>>();
            var allShows = await repo.GetAsync(
                show => show.Date > new DateTimeOffset(new DateTime(2018, 1, 1)));

            var options =
                serviceProvider
                    .GetRequiredService<IOptions<RepositoryOptions>>()
                    .Value;

            var connectionString = "AccountEndpoint=https://dotnetdocsshowdata.documents.azure.com:443/;AccountKey=ipPIJ4ikE1pFIRoP1ujzCO10oagAX3nStXtOiOzyXxCJ4m9IOXHXscMH82Wbt9xW1XlyXZ3wxnp4KZGhoVNhvg==;";
            using var cosmosClient = new CosmosClient(connectionString);
            Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(options.DatabaseId);

            var properties = new ContainerProperties
            {
                Id = options.ContainerId,
                PartitionKeyPath = "/id"
            };
            Container container = await database.CreateContainerIfNotExistsAsync(properties);

            foreach (var show in allShows)
            {
                if (show.ShowImage?.Contains("dotnetdocsstorage") ?? false)
                {

                    show.ShowImage = show.ShowImage.Replace("dotnetdocsstorage", "dotnetdocsshowstorage");

                    await container.UpsertItemAsync(show);
                    Console.WriteLine($"Updated '{show.Title}' ({show.Id})");
                }
            }
        }
    }
}
