using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;

namespace PartyGames.Web.Infrastructure.Layout
{
    public class PageHeadBuilder : IPageHeadBuilder
    {
        private readonly List<string> _titleParts;
        private readonly List<string> _scriptParts;
        private readonly List<string> _styleParts;

        private readonly HttpContextBase _http;

        public PageHeadBuilder(HttpContextBase http)
        {
            _http = http;
            _titleParts = new List<string>();
            _scriptParts = new List<string>();
            _styleParts = new List<string>();
        }

        public void AddTitleParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _titleParts.Add(part);
        }

        public void AppendTitleParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _titleParts.Insert(0, part);
        }

        public string GenerateTitle()
        {
            return string.Join(" - ", _titleParts);
        }

        public void AddScriptPart(string path)
        {
            _scriptParts.Add(path);
        }

        public void AppendScriptPart(string path)
        {
            _scriptParts.Insert(0, path);
        }

        public MvcHtmlString GenerateScripts()
        {
            var isLocalRequest = _http?.Request.IsLocal ?? false;
            BundleTable.EnableOptimizations = !isLocalRequest;

            var result = new StringBuilder();

            foreach (var src in _scriptParts)
            {
                result.Append(Scripts.Render(src));
                //result.AppendLine();
                //result.AppendFormat("<script src=\"{0}\" type=\"application/javascript\"></script>", src);
            }

            return MvcHtmlString.Create(result.ToString());
        }

        public void AddStylePart(string path)
        {
            _styleParts.Add(path);
        }

        public void AppendStylePart(string path)
        {
            _styleParts.Insert(0, path);
        }

        public MvcHtmlString GenerateStyles()
        {
            var isLocalRequest = _http?.Request.IsLocal ?? false;
            BundleTable.EnableOptimizations = !isLocalRequest;

            var result = new StringBuilder();
            
            foreach (var src in _styleParts)
            {
                result.Append(Styles.Render(src));
                //result.AppendLine();
                //result.AppendFormat("<link rel=\"stylesheet\" href=\"{0}\" />", src);
            }

            return MvcHtmlString.Create(result.ToString());
        }
    }
}