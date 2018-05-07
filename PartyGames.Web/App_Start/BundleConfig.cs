using System.Web;
using System.Web.Optimization;

namespace PartyGames.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/core/js").Include(
                "~/Asserts/libs/jquery-3.3.1/jquery-3.3.1.min.js",
                "~/Asserts/libs/fontawesome-free-5.0.7/svg-with-js/js/fontawesome-all.min.js",
                "~/Asserts/libs/bootstrap-3.3.7/js/bootstrap.min.js",
                "~/Asserts/libs/helpers/cookie.js",
                "~/Asserts/libs/helpers/querystring.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/core/css").Include(
                "~/Asserts/libs/fontawesome-free-5.0.7/svg-with-js/css/fa-svg-with-js.css",
                "~/Asserts/libs/bootstrap-3.3.7/bootstrap.min.css",
                "~/Asserts/libs/animate.css-3.5.1/animate.min.css"));

            bundles.Add(new StyleBundle("~/bundles/home/css").Include("~/Asserts/css/home.css"));
            bundles.Add(new StyleBundle("~/bundles/mc/css").Include("~/Asserts/css/mc.css"));
            bundles.Add(new StyleBundle("~/bundles/bingo/css").Include("~/Asserts/css/bingo.css"));

            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));


        }
    }
}
