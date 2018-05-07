using System;
using System.Collections.Generic;
using PartyGames.Service.Bingo;

namespace PartyGames.Web.Models.Bingo
{
    public class PlayerModel
    {
        public PlayerModel()
        {
            GameMark = new GameMarks();
        }

        public string CardName { get; set; }
        public int PlayerId { get; set; }
        public int GameId { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public DateTime? LastUpdate { get; set; }
        public DateTime CreateDate { get; set; }
        public string CardUniqueNo { get; set; }
        public List<int> CardNumbers { get; set; }

        public GameMarks GameMark { get; set; }
    }
}