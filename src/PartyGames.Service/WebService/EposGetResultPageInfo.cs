using Newtonsoft.Json;

namespace PartyGames.Service.WebService
{
    public class EposGetResultPageInfo
    {
        public int Page { get; set; }

        [JsonProperty("page_size")]
        public int PageSize { get; set; }

        [JsonProperty("total_page")]
        public int TotalPage { get; set; }

        [JsonProperty("total_records")]
        public int TotalRecords { get; set; }
    }
}