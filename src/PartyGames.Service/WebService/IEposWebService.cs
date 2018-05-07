using PartyGames.Service.WebService.Models;

namespace PartyGames.Service.WebService
{
    public interface IEposWebService
    {
        EposGetResult<EposBingoCard> GetBingoCards();
        EposGetResult<EposMcGame> GetMcGames();
        EposGetResult<EposMcQuestion> GetMcQuestions(string user, string type, string level, string mode = "P2W");
    }
}