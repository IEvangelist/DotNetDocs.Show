namespace DotNetDocs.Repository.Options
{
    public class RepositoryOptions
    {
        public string CosmosConnectionString { get; set; } = null!;

        public string DatabaseId { get; set; } = null!;

        public string ContainerId { get; set; } = null!;
    }
}