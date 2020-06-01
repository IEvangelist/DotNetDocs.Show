using Microsoft.Azure.Cosmos;

namespace DotNetDocs.Repository.Providers
{
    public interface ICosmosContainerProvider
    {
        Container GetContainer();
    }
}