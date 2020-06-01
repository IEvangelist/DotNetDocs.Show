using System;
using DotNetDocs.Repository.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace DotNetDocs.Repository.Providers
{
    public class CosmosContainerProvider : ICosmosContainerProvider, IDisposable
    {
        readonly RepositoryOptions _options;
        readonly Lazy<Container> _lazyContainer = null!;

        CosmosClient _client = null!;

        public CosmosContainerProvider(
            IOptions<RepositoryOptions> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _lazyContainer = new Lazy<Container>(() =>
            {
                _client = new CosmosClient(_options.CosmosConnectionString);
                var database = _client.GetDatabase(_options.DatabaseId);
                return database.GetContainer(_options.ContainerId);
            });
        }

        public Container GetContainer() => _lazyContainer.Value;

        public void Dispose() => _client?.Dispose();
    }
}