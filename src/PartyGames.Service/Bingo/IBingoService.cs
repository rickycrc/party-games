using System.Collections.Generic;
using PartyGames.Data.BingoContext;

namespace PartyGames.Service.Bingo
{
    public interface IBingoService
    {
        List<Card> GetCards();
        void ResetBingo(bool resetCard);
        List<int> GetCachedLiveGameRolledNumbers();
        Game GetLiveGame();
        Game GetGameByCode(string code);
        Card GetCardByCardNo(string cardNo);
        int RollNewNumber(Game game);
        Card GetNewUniqueCard();
    }
}