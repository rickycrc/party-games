namespace PartyGames.Web.Models.Common
{
    public class PanelMessageModel
    {
        public PanelMessageModel(PanelMessageType type, string message)
        {
            PanelMessageType = type;
            Message = message;
        }

        public PanelMessageType PanelMessageType { get; set; }
        public string Message { get; set; }
    }
}