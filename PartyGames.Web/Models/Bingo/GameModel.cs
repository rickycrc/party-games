using System;
using System.Collections.Generic;

namespace PartyGames.Web.Models.Bingo
{
    public class GameModel
    {
        public int GameId { get; set; }
        public string Code { get; set; }
        public string Password { get; set; }
        public DateTime CreateDate { get; set; }
        public List<int> RolledNumbers { get; set; }
    }
}