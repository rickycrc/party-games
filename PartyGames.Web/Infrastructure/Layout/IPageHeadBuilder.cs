using System.Web.Mvc;

namespace PartyGames.Web.Infrastructure.Layout
{
    public interface IPageHeadBuilder
    {
        void AddTitleParts(string part);
        void AppendTitleParts(string part);
        string GenerateTitle();
        void AddScriptPart(string path);
        void AppendScriptPart(string path);
        MvcHtmlString GenerateScripts();
        void AddStylePart(string path);
        void AppendStylePart(string path);
        MvcHtmlString GenerateStyles();
    }
}