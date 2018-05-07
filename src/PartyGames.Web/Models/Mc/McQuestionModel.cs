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

        public Dictionary<string, string> McAnswers
        {
            get
            {
                var result = new Dictionary<string, string>();
                if (string.IsNullOrEmpty(McAnswersText) || string.IsNullOrEmpty(McAnswersValue))
                    return result;

                var mt = McAnswersText.Split(',');
                var mv = McAnswersValue.Split(',');

                if (mt.Length != mv.Length)
                    return result;

                for (int i = 0; i < mt.Length; i++)
                {
                    var code = mv[i].Trim();
                    var text = mt[i].Trim();

                    if (string.IsNullOrEmpty(McAnswersText) || string.IsNullOrEmpty(McAnswersValue))
                        continue;

                    result.Add(code, text);
                }

                return result;
            }
        }
    }
}