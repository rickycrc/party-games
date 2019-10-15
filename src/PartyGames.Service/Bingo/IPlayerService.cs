using System.Collections.Generic;
using PartyGames.Data.BingoContext;

namespace PartyGames.Service.Bingo
{
    public interface IPlayerService
    {
        void AddTestingPlayer(Game game, int count);
        Player AddPlayerToGame(Game game, string name, Card card = null);
        void UpdatePlayer(Player player);
        void DeletePlayer(string key);
        Player GetPlayerByKey(string key);
        Player GetCachedPlayer(string key);
        IList<Player> GetPlayersByGame(Game game);
        Player AssignPlayerToCard(Game game, string name, Card card);
        Player GetPlayerByCard(Game game, string cardNo, string checkDigit);
    }
}