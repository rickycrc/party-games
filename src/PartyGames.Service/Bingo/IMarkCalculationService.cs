using PartyGames.Data.BingoContext;

namespace PartyGames.Service.Bingo
{
    public interface IMarkCalculationService
    {
        GameMarks CalculateGameMark(Game game, Player player);
    }
}