namespace PartyGames.Web.Models.Bingo
{
    public class RankingPlayerModel
    {
        public int Ranking { get; set; }
        public string CardNo { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public int BingoMark1 { get; set; }
        public int BingoMark2 { get; set; }
        public int BingoMark3 { get; set; }
        public bool Active { get; set; }
    }
}