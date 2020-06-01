using DotNetDocs.Repository.Providers;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DotNetDocs.Repository
{
    public class Repository<T> : IRepository<T> where T : Document
    {
        readonly ICosmosContainerProvider _containerProvider;
        readonly ILogger<T> _logger;

        public Repository(
            ICosmosContainerProvider containerProvider,
            ILogger<T> logger) =>
            (_containerProvider, _logger) = (containerProvider, logger);

        public async ValueTask<T?> GetAsync(string id)
        {
            try
            {
                Container? container = _containerProvider.GetContainer();
                ItemResponse<T>? response = await container.ReadItemAsync<T>(id, new PartitionKey(id));

                return response.Resource;
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex.Message, ex);
                return default;
            }
        }

        public async ValueTask<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                var iterator =
                    _containerProvider.GetContainer()
                                      .GetItemLinqQueryable<T>()
                                      .Where(predicate)
                                      .ToFeedIterator();

                var results = new List<T>();
                while (iterator.HasMoreResults)
                {
                    foreach (T result in await iterator.ReadNextAsync())
                    {
                        results.Add(result);
                    }
                }

                return results;
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex.Message, ex);
                return Enumerable.Empty<T>();
            }
        }

        public async ValueTask<T> CreateAsync(T value)
        {
            Container? container = _containerProvider.GetContainer();
            ItemResponse<T>? response = await container.CreateItemAsync(value, value.PartitionKey);

            return response.Resource;
        }

        public Task<T[]> CreateAsync(IEnumerable<T> values) =>
            Task.WhenAll(values.Select(v => CreateAsync(v).AsTask()));

        public async ValueTask<T> UpdateAsync(T value)
        {
            Container? container = _containerProvider.GetContainer();
            ItemResponse<T>? response = await container.UpsertItemAsync<T>(value, value.PartitionKey);

            return response.Resource;
        }

        public async ValueTask<T> DeleteAsync(string id)
        {
            Container? container = _containerProvider.GetContainer();
            ItemResponse<T>? response = await container.DeleteItemAsync<T>(id, new PartitionKey(id));

            return response.Resource;
        }
    }
}
