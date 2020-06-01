using DotNetDocs.Repository.Providers;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace DotNetDocs.Repository
{
    public class Repository<T> : IRepository<T> where T : Document
    {
        readonly ICosmosContainerProvider _containerProvider;

        public Repository(
            ICosmosContainerProvider containerProvider) =>
            _containerProvider = containerProvider ?? throw new ArgumentNullException(nameof(containerProvider));

        public async ValueTask<T?> GetAsync(string id)
        {
            try
            {
                var container = _containerProvider.GetContainer();
                var response = await container.ReadItemAsync<T>(id, new PartitionKey(id));

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
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
                    foreach (var result in await iterator.ReadNextAsync())
                    {
                        results.Add(result);
                    }
                }

                return results;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return Enumerable.Empty<T>();
            }
        }

        public async ValueTask<T> CreateAsync(T value)
        {
            var container = _containerProvider.GetContainer();
            var response = await container.CreateItemAsync(value, value.PartitionKey);

            return response.Resource;
        }

        public Task<T[]> CreateAsync(IEnumerable<T> values) =>
            Task.WhenAll(values.Select(v => CreateAsync(v).AsTask()));

        public async ValueTask<T> UpdateAsync(T value)
        {
            var container = _containerProvider.GetContainer();
            var response = await container.UpsertItemAsync<T>(value, value.PartitionKey);

            return response.Resource;
        }

        public async ValueTask<T> DeleteAsync(string id)
        {
            var container = _containerProvider.GetContainer();
            var response = await container.DeleteItemAsync<T>(id, new PartitionKey(id));

            return response.Resource;
        }
    }
}