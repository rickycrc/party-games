using Newtonsoft.Json;

namespace PartyGames.Service.WebService.Models
{
    public class EposBingoCard : IEposData
    {
        [JsonProperty("cardid")]
        public string CardNo { get; set; }

        [JsonProperty("cardtype")]
        public string CardType { get; set; }

        [JsonProperty("longstr")]
        public string Numbers { get; set; }
    }
}