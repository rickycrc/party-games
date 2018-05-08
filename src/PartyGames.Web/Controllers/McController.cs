using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using HtmlAgilityPack;
using PartyGames.Service.WebService;
using PartyGames.Web.Models.Mc;

namespace PartyGames.Web.Controllers
{

    [RoutePrefix("mc")]
    public class McController : Controller
    {
        private readonly IEposWebService _eposWebService;

        public McController(IEposWebService eposWebService)
        {
            _eposWebService = eposWebService;
        }


        [NonAction]
        protected string GetUserOrCreateIfNotExits()
        {
            //check cookie exists
            if (Request.Cookies["user"] == null)
            {
                var guid = Guid.NewGuid();
                //add new cookie to user browser
                var cookie = new HttpCookie("user")
                {
                    Value = guid.ToString(),
                    Expires = DateTime.Now.AddDays(30),
                    HttpOnly = true
                };
                Response.Cookies.Add(cookie);

                return guid.ToString();
            }

            return Request.Cookies["user"].ToString();
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [Route("play/{urlalias}")]
        public ActionResult Play(string urlalias)
        {

            if (string.IsNullOrWhiteSpace(urlalias))
                return RedirectToAction("Index", "Home");

            var user = GetUserOrCreateIfNotExits();
            var mcGames = _eposWebService.GetMcGames();
            var mcGame = mcGames.Data.FirstOrDefault(c => c.UrlAlias == urlalias);

            //return to home if mc game doesn't exists
            if (mcGame == null)
                return RedirectToAction("Index", "Home");

            var model = new GamePlayModel
            {
                Game = Mapper.Map(mcGame, new McGameModel())
            };

            return View(model);
        }


        [HttpPost]
        public ActionResult GetQuestions(string type, string level)
        {
            try
            {
                //ServicePointManager.Expect100Continue = true;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var user = GetUserOrCreateIfNotExits();
                var question = _eposWebService.GetMcQuestions(user, type, level);

                var model = Mapper.Map(question.Data, new List<McQuestionModel>());
                return PartialView("_Quiz", model);
            }
            catch (Exception ex)
            {
                var html = "<div class='panel-play__error'>An error occured.<br />Message: " + ex.ToString() + "</div>";
                return Content(html, "text/html;", Encoding.UTF8);
            }
        }

        #region web API

        private static object _rLock = new object();

        [Route("get-sound-link"), HttpGet]
        public ActionResult GetSoundLinkFromCambridge(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            lock (_rLock)
            {
                word = word.ToLower().Trim();
                //var url = $"https://dictionary.cambridge.org/dictionary/english/{word}";
                var url = $"https://dictionary.cambridge.org/search/english/direct/?q={word}";
                try
                {
                    var request = WebRequest.Create(url) as HttpWebRequest;

                    request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                    request.Host = "dictionary.cambridge.org";
                    request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.117 Safari/537.36";

                    request.Headers.Add(HttpRequestHeader.AcceptLanguage, "zh-TW,zh;q=0.9,en-US;q=0.8,en;q=0.7,ko;q=0.6,zh-CN;q=0.5,ja;q=0.4");
                    request.Headers.Add(HttpRequestHeader.CacheControl, "no-cache");
                    request.Headers.Add(HttpRequestHeader.Pragma, "no-cache");
                    request.Headers.Add(HttpRequestHeader.Upgrade, "1");

                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                    var html = string.Empty;

                    using (var response = request.GetResponse() as HttpWebResponse)
                    {
                        var sb = new StringBuilder();
                        var buf = new byte[8192];
                        using (var resStream = response.GetResponseStream())
                        {
                            int count = 0;

                            do
                            {
                                count = resStream.Read(buf, 0, buf.Length);
                                if (count != 0)
                                {
                                    sb.Append(Encoding.UTF8.GetString(buf, 0, count)); // just hardcoding UTF8 here
                                }
                            } while (count > 0);

                            html = sb.ToString();
                        }
                    }

                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    var node = doc.DocumentNode.Descendants(0).FirstOrDefault(c => c.HasClass("audio_play_button") && c.HasClass("uk"));


                    if (node == null)
                        node = doc.DocumentNode.Descendants(0).FirstOrDefault(c => c.HasClass("audio_play_button"));

                    if (node == null)
                        throw new Exception("Cannot find docuemnt node with class 'audio_play_button' & 'uk/us'");



                    return Json(new
                    {
                        Success = true,
                        Message = "Success",
                        //Country = node.HasClass("uk") ? "uk" : "us",
                        Country = node.ParentNode.Descendants("span").FirstOrDefault(c => c.HasClass("region"))?.InnerHtml,
                        Mp3 = node.GetAttributeValue("data-src-mp3", null),
                        Ogg = node.GetAttributeValue("data-src-ogg", null),
                        InquiryTime = DateTime.Now.ToString("O")
                    }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception message)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = message.Message,
                        InquiryTime = DateTime.Now.ToString("O")
                    }, JsonRequestBehavior.AllowGet);
                }
            }

        }

        #endregion
    }
}