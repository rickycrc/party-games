using System.Collections.Generic;

namespace PartyGames.Service.Bingo
{
    public class GameMarks
    {
        public GameMarks()
        {
            Mark3 = new List<int>();
        }

        public int Mark1 { get; set; }
        public int Mark2 { get; set; }
        public List<int> Mark3 { get; set; }
    }
}