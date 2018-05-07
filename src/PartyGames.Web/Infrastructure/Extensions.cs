using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using PartyGames.Data.BingoContext;
using PartyGames.Service.Bingo;
using PartyGames.Web.Models.Bingo;

namespace PartyGames.Web.Infrastructure
{
    public static class Extensions
    {
        public static List<PlayerModel> ToModels(this IList<Player> players, Game game)
        {
            var bingoService = DependencyResolver.Current.GetService<IBingoService>();
            var markCalculationService = DependencyResolver.Current.GetService<IMarkCalculationService>();
            var result = new List<PlayerModel>();

            if (players == null)
                return result;

            foreach (var player in players)
            {
                var playerModel = Mapper.Map(player, new PlayerModel());
                playerModel.GameMark = markCalculationService.CalculateGameMark(game, player);

                result.Add(playerModel);
            }

            return result;
        }

        public static List<PlayerModel> SortByMarks(this List<PlayerModel> models)
        {
            var sortedPlayers = models
                .OrderBy(c => !c.GameMark.Mark3.Any())
                .ThenBy(c => c.GameMark.Mark3.FirstOrDefault())
                .ThenByDescending(c => c.GameMark.Mark1)
                .ThenByDescending(c => c.GameMark.Mark3.Count > 1 ? 1 : c.GameMark.Mark2)
                .ThenByDescending(c => c.CreateDate);

            return sortedPlayers.ToList();
        }
    }
}