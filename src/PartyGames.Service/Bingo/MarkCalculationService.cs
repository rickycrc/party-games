using System.Collections.Generic;
using System.Linq;
using PartyGames.Data.BingoContext;

namespace PartyGames.Service.Bingo
{
    public class MarkCalculationService : IMarkCalculationService
    {
        public GameMarks CalculateGameMark(Game game, Player player)
        {
            var rolledNumbers = game.GetRolledNumberList();
            var cardNumbers = player.GetNumberList();

            var marks = new GameMarks();
            var lineMatchCount = new List<int>();
            var bingoCellIndex = GetBingoCellIndex();

            //loop 12 lines
            foreach (var cellIdxs in bingoCellIndex)
            {
                var lineMatch = cellIdxs.Count(idx => rolledNumbers.Contains(cardNumbers[idx]) || cardNumbers[idx] == 0);
                lineMatchCount.Add(lineMatch);
            }

            marks.Mark1 = lineMatchCount.Max();
            marks.Mark2 = marks.Mark1 == 0 ? 0 : lineMatchCount.Count(c => c == marks.Mark1);
            //marks.Mark3 = 0;

            //marks 3
            if (marks.Mark1 >= 5)
            {
                var bingoLine = new List<int>();

                for (var i = 0; i < rolledNumbers.Count; i++)
                    //loop 12 lines
                    for (var j = 0; j < bingoCellIndex.Count; j++)
                    {
                        var cellIdxs = bingoCellIndex[j];
                        var lineMatch = cellIdxs.Count(idx => rolledNumbers.Take(i + 1).Contains(cardNumbers[idx]) || cardNumbers[idx] == 0);

                        //bingo 5 number in a row
                        if (lineMatch == 5 && !bingoLine.Contains(j))
                        {
                            //add roll number to marks
                            marks.Mark3.Add(i + 1);
                            bingoLine.Add(j);
                        }
                    }
            }

            return marks;
        }

        private List<int[]> GetBingoCellIndex()
        {
            var result = new List<int[]>();

            //row
            result.Add(new[] { 0, 1, 2, 3, 4 });
            result.Add(new[] { 5, 6, 7, 8, 9 });
            result.Add(new[] { 10, 11, 12, 13, 14 });
            result.Add(new[] { 15, 16, 17, 18, 19 });
            result.Add(new[] { 20, 21, 22, 23, 24 });


            //column
            result.Add(new[] { 0, 5, 10, 15, 20 });
            result.Add(new[] { 1, 6, 11, 16, 21 });
            result.Add(new[] { 2, 7, 12, 17, 22 });
            result.Add(new[] { 3, 8, 13, 18, 23 });
            result.Add(new[] { 4, 9, 14, 19, 24 });


            //cross
            result.Add(new[] { 0, 6, 12, 18, 24 });
            result.Add(new[] { 4, 8, 12, 16, 20 });


            return result;
        }
    }
}
