using Newtonsoft.Json;
using PartyGames.Service.WebService.Models;

namespace PartyGames.Service.WebService
{
    public class EposWebService : IEposWebService
    {

        public EposGetResult<EposBingoCard> GetBingoCards()
        {
            //get cards from epos
            var url = "https://eposws.ascotchang.com/epos_ajax_read.avfp?type=Get-bingo-card&token=23644384&seokey=5-5-500";
            var request = new EposWebClient();
            var response = request.DownloadString(url);

            var result = JsonConvert.DeserializeObject<EposGetResult<EposBingoCard>>(response);

            return result;
        }

        public EposGetResult<EposMcGame> GetMcGames()
        {
            //get cards from epos
            var url = "https://eposws.ascotchang.com/epos_ajax_read.avfp?token=23644384&type=GET-GAME-MENU";
            var request = new EposWebClient();
            var response = request.DownloadString(url);

            var result = JsonConvert.DeserializeObject<EposGetResult<EposMcGame>>(response);

            return result;
        }

        public EposGetResult<EposMcQuestion> GetMcQuestions(string user, string type, string level, string mode = "P2W")
        {
            //get cards from epos
            var url = $"https://eposws.ascotchang.com/epos_ajax_read.avfp?token=23644384&type={type}&seoKey={level}&seokey2={mode}&user={user}";
            var request = new EposWebClient();
            var response = request.DownloadString(url);

            var result = JsonConvert.DeserializeObject<EposGetResult<EposMcQuestion>>(response);

            return result;
        }

    }
}
