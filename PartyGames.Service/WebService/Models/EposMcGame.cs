using Newtonsoft.Json;

namespace PartyGames.Service.WebService.Models
{
    public class EposMcGame : IEposData
    {
        [JsonProperty("apitype")]
        public string ApiType { get; set; }
        [JsonProperty("apicaption")]
        public string Caption { get; set; }
        [JsonProperty("btncol")]
        public string ButtonColor { get; set; }
        [JsonProperty("btnbgcol")]
        public string ButtonBackgroundColor { get; set; }
        [JsonProperty("urlalias")]
        public string UrlAlias { get; set; }
        
    }
}