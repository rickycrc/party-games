using System.Collections.Generic;

namespace PartyGames.Web.Models.Mc
{
    public class McQuestionModel
    {
        public double QuestionNo { get; set; }
        public string ImagePath { get; set; }
        public string AudioPath { get; set; }
        public string Answer { get; set; }
        public string McAnswersText { get; set; }
        public string McAnswersValue { get; set; }
        public string McAnswersAudio { get; set; }

        public List<McAnswerModel> McAnswers
        {
            get
            {
                var result = new List<McAnswerModel>();

                if (string.IsNullOrEmpty(McAnswersText) || string.IsNullOrEmpty(McAnswersValue) || string.IsNullOrEmpty(McAnswersAudio))
                    return result;

                var mt = McAnswersText.Split(',');
                var mv = McAnswersValue.Split(',');
                var ma = McAnswersAudio.Split(',');

                if (mt.Length != mv.Length)
                    return result;

                for (int i = 0; i < mt.Length; i++)
                {
                    var code = mv[i].Trim();
                    var text = mt[i].Trim();
                    var audio = i < ma.Length ? ma[i].Trim() : null;

                    if (audio == null || audio == "MediaFile-NotFound")
                        audio = "";

                    if (string.IsNullOrEmpty(McAnswersText) || string.IsNullOrEmpty(McAnswersValue))
                        continue;

                    result.Add(new McAnswerModel
                    {
                        Text = text,
                        Value = code,
                        Mp3 = audio
                    });
                }

                return result;
            }
        }
    }
}