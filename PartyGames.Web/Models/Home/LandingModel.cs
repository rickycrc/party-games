using System.Collections.Generic;
using PartyGames.Web.Models.Mc;

namespace PartyGames.Web.Models.Home
{
    public class LandingModel
    {
        public LandingModel()
        {
            McGames = new List<McGameModel>();
        }
        public List<McGameModel> McGames { get; set; }
    }
}