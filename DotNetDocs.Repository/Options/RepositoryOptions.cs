namespace DotNetDocs.Repository.Options
{
    public class RepositoryOptions
    {
        public string CosmosConnectionString { get; set; } = null!;

        public string DatabaseId { get; set; } = "DotNetDocs";

        public string ContainerId { get; set; } = "Development";
    }
}
