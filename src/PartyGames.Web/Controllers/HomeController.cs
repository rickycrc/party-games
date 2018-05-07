using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using PartyGames.Service.WebService;
using PartyGames.Web.Models.Home;
using PartyGames.Web.Models.Mc;

namespace PartyGames.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEposWebService _eposWebService;

        public HomeController(IEposWebService eposWebService)
        {
            _eposWebService = eposWebService;
        }

        public ActionResult Index()
        {
            var mcGames = _eposWebService.GetMcGames();
            var mcGamesModel = Mapper.Map(mcGames.Data, new List<McGameModel>());

            var model = new LandingModel();

            model.McGames.AddRange(mcGamesModel);

            return View(model);
        }


    }
}