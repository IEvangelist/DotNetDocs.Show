using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;

namespace DotNetDocs.Repository
{
    public class Document
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        internal PartitionKey PartitionKey => new PartitionKey(Id);
    }
}