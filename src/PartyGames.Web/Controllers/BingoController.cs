using System.Web.Mvc;
using AutoMapper;
using PartyGames.Service.Bingo;
using PartyGames.Web.Infrastructure;
using PartyGames.Web.Models.Bingo;
using PartyGames.Web.Models.Common;

namespace PartyGames.Web.Controllers
{
    public class BingoController : BaseController
    {
        private readonly IBingoService _bingoService;
        private readonly IPlayerService _playerService;

        public BingoController(IBingoService bingoService,
            IPlayerService playerService)
        {
            _bingoService = bingoService;
            _playerService = playerService;
        }

        public ActionResult Index()
        {
            return RedirectToAction("Play");
        }


        public ActionResult Play()
        {
            return View();
        }

        public ActionResult MockPlay()
        {
            return View();
        }


        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult Admin(AdminModel model)
        {
            var game = _bingoService.GetLiveGame();
            var players = _playerService.GetPlayersByGame(game).ToModels(game).SortByMarks();

            model.Game = Mapper.Map(game, new GameModel());
            model.Players = players;

            return View(model);
        }

        public ActionResult AdminRoll()
        {
            var game = _bingoService.GetLiveGame();
            var newNumber = _bingoService.RollNewNumber(game);

            return RedirectToAction("Admin");
        }

        public ActionResult AdminReset(bool? resetCard)
        {
            if (!resetCard.HasValue)
                return RedirectToAction("Admin");

            _bingoService.ResetBingo(resetCard.Value);
            var game = _bingoService.GetLiveGame();
            //_bingoService.AddTestingPlayer(game, 80);

            return RedirectToAction("Admin");
        }

        public ActionResult AdminDeletePlayer(string key)
        {
            if (!string.IsNullOrEmpty(key))
                _playerService.DeletePlayer(key);

            return RedirectToAction("Admin");
        }

        [HttpPost]
        public ActionResult AdminNewCard(NewCardModel newCard)
        {
            var card = _bingoService.GetCardByCardNo(newCard.No);
            if (card == null || card.CardType != "MANUAL" || card.Used)
            {
                AddPanelMessage(new PanelMessageModel(PanelMessageType.danger, "Invalid card no. #" + card?.CardNo));
                return RedirectToAction("Admin", newCard);
            }

            if (string.IsNullOrEmpty(newCard.Name))
                newCard.Name = card.CardNo;

            var game = _bingoService.GetLiveGame();
            var player = _playerService.AssignPlayerToCard(game, newCard.Name, card);

            AddPanelMessage(new PanelMessageModel(PanelMessageType.success, $"Success! {player.Name} has been assigned to card #{newCard.No}."));

            return RedirectToAction("Admin");
        }
    }
}