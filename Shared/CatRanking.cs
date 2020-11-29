using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace CatMash.Shared
{
    public class CatRanking
    {
        [JsonProperty(PropertyName = "id")]
        [JsonPropertyName("id")]
        public string Id => CatId;

        [JsonProperty(PropertyName = "catid")]
        [JsonPropertyName("catid")]
        public string CatId { get; set; }

        [JsonProperty(PropertyName = "counter")]
        [JsonPropertyName("counter")]
        public int VoteCount { get; set; }

        [JsonProperty(PropertyName = "imageurl")]
        [JsonPropertyName("imageurl")]
        public string ImageUrl { get; set; }

        public string PartitionKey => this.CatId;

        public static string PartitionKeyPath => "/catid";
    }
}
