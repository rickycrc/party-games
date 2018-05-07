using System.Collections.Generic;
using Newtonsoft.Json;

namespace PartyGames.Service.WebService
{
    public class EposGetResult<T>
    {
        public EposGetResult()
        {
            //Header = new List<EposGetResultHeader>();
            Data = new List<T>();
            PageInfo = new EposGetResultPageInfo();
        }

        //[JsonProperty("header")]
        //public List<EposGetResultHeader> Header { get; set; }
        [JsonProperty("data")]
        public List<T> Data { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("rtnmessage")]
        public string ReturnMessage { get; set; }

        [JsonProperty("msgcode")]
        public string MessageCode { get; set; }

        [JsonProperty("pageinfo")]
        public EposGetResultPageInfo PageInfo { get; set; }
    }
}