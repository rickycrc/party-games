using System.Text;
using System.Web.Mvc;

namespace PartyGames.Web.Infrastructure.Layout
{
    public static class LayoutExtentions
    {
        public static MvcHtmlString GenerateFavicon(this HtmlHelper html)
        {
            var result = new StringBuilder();
            result.AppendLine("<link rel=\"apple-touch-icon\" sizes=\"120x120\" href=\"/apple-touch-icon.png\">");
            result.AppendLine("<link rel=\"icon\" type=\"image/png\" sizes=\"32x32\" href=\"/favicon-32x32.png\">");
            result.AppendLine("<link rel=\"icon\" type=\"image/png\" sizes=\"16x16\" href=\"/favicon-16x16.png\">");
            result.AppendLine("<link rel=\"manifest\" href=\"/site.webmanifest\">");
            result.AppendLine("<link rel=\"mask-icon\" href=\"/safari-pinned-tab.svg\" color=\"#5bbad5\">");
            result.AppendLine("<meta name=\"msapplication-TileColor\" content=\"#603cba\">");
            result.AppendLine("<meta name=\"theme-color\" content=\"#ffffff\">");
            return MvcHtmlString.Create(result.ToString());
        }

        public static void AddTitleParts(this HtmlHelper html, string part)
        {
            var pageHeadBuilder = DependencyResolver.Current.GetService<IPageHeadBuilder>();
            pageHeadBuilder.AddTitleParts(part);
        }

        public static void AppendTitleParts(this HtmlHelper html, string part)
        {
            var pageHeadBuilder = DependencyResolver.Current.GetService<IPageHeadBuilder>();
            pageHeadBuilder.AppendTitleParts(part);
        }

        public static string GenerateTitle(this HtmlHelper html)
        {
            var pageHeadBuilder = DependencyResolver.Current.GetService<IPageHeadBuilder>();
            return pageHeadBuilder.GenerateTitle();
        }

        public static void AddScriptPart(this HtmlHelper html, string path)
        {
            var pageHeadBuilder = DependencyResolver.Current.GetService<IPageHeadBuilder>();
            pageHeadBuilder.AddScriptPart(path);
        }

        public static void AppendScriptPart(this HtmlHelper html, string path)
        {
            var pageHeadBuilder = DependencyResolver.Current.GetService<IPageHeadBuilder>();
            pageHeadBuilder.AppendScriptPart(path);
        }

        public static MvcHtmlString GenerateScripts(this HtmlHelper html)
        {
            var pageHeadBuilder = DependencyResolver.Current.GetService<IPageHeadBuilder>();
            return pageHeadBuilder.GenerateScripts();
        }

        public static void AddStylePart(this HtmlHelper html, string path)
        {
            var pageHeadBuilder = DependencyResolver.Current.GetService<IPageHeadBuilder>();
            pageHeadBuilder.AddStylePart(path);
        }

        public static void AppendStylePart(this HtmlHelper html, string path)
        {
            var pageHeadBuilder = DependencyResolver.Current.GetService<IPageHeadBuilder>();
            pageHeadBuilder.AppendStylePart(path);
        }

        public static MvcHtmlString GenerateStyles(this HtmlHelper html)
        {
            var pageHeadBuilder = DependencyResolver.Current.GetService<IPageHeadBuilder>();
            return pageHeadBuilder.GenerateStyles();
        }
    }
}