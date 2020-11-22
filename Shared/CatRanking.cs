using Newtonsoft.Json;

namespace CatMash.Shared
{
    public class CatRanking
    {
        [JsonProperty(PropertyName = "catid")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "counter")]
        public int VoteCount { get; set; }
    }
}
