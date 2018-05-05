using System.Net;
using System.Text;

namespace PartyGames.Service.WebService
{
    public class EposWebClient : WebClient
    {
        public EposWebClient()
        {
            base.Encoding = Encoding.UTF8;
        }
    }
}
