using Newtonsoft.Json;

namespace PartyGames.Service.WebService.Models
{

    public class EposMcQuestion : IEposData
    {
        [JsonProperty("quiz_seq")]
        public double QuestionNo { get; set; }
        [JsonProperty("xx_img")]
        public string ImagePath { get; set; }
        [JsonProperty("xx_audio")]
        public string AudioPath { get; set; }
        [JsonProperty("quiz_ans")]
        public string Answer { get; set; }
        [JsonProperty("mc_seokeys")]
        public string McAnswersText { get; set; }
        [JsonProperty("mc_codeid")]
        public string McAnswersValue { get; set; }
    }
}