using Newtonsoft.Json;

namespace CatMash.Shared
{
    public class CatRanking
    {
        [JsonProperty(PropertyName = "id")]
        public string Id => CatId;

        [JsonProperty(PropertyName = "catid")]
        public string CatId { get; set; }

        [JsonProperty(PropertyName = "counter")]
        public int VoteCount { get; set; }

        [JsonProperty(PropertyName = "imageurl")]
        public string ImageUrl { get; set; }

        public string PartitionKey => this.CatId;

        public static string PartitionKeyPath => "/catid";
    }
}
