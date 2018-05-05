using System.Collections.Generic;

namespace PartyGames.Web.Models.Bingo
{
    public class AdminModel
    {
        public GameModel Game { get; set; }
        public List<PlayerModel> Players { get; set; }
    }
}