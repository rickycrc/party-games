using System;
using System.Collections.Generic;
using System.Linq;
using PartyGames.Data.BingoContext;

namespace PartyGames.Service.Bingo
{
    public static class BingoExtensions
    {
        private static List<int> ConvertToNumberList(string numbers)
        {
            return numbers.Split(',').Select(c => Convert.ToInt32(c)).ToList();
        }

        public static List<int> GetRolledNumberList(this Game game)
        {
            if (game == null)
                return null;

            if (string.IsNullOrEmpty(game.RolledNumbers))
                return new List<int>();

            return ConvertToNumberList(game.RolledNumbers);
        }

        public static List<int> GetNumberList(this Player player)
        {
            if (player == null)
                return null;

            if (string.IsNullOrEmpty(player.CardNumbers))
                return new List<int>();

            return ConvertToNumberList(player.CardNumbers);
        }

        public static int[,] GetCardNumbers(this Player player, int itemPerColumn = 5, int itemPerRow = 5)
        {
            if (string.IsNullOrEmpty(player?.CardNumbers))
                return null;

            var numberList = ConvertToNumberList(player.CardNumbers);
            var cardNumbers = new int[itemPerColumn, itemPerRow];

            if (numberList.Count != itemPerColumn * itemPerRow)
                return null;
            var pos = 0;
            for (var i = 0; i < itemPerColumn; i++)
                for (var j = 0; j < itemPerRow; j++)
                {
                    cardNumbers[i, j] = numberList[pos];
                    pos++;
                }

            return cardNumbers;
        }
    }
}