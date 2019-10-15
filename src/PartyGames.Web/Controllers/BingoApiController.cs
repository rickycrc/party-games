using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using AutoMapper;
using PartyGames.Service.Bingo;
using PartyGames.Web.Infrastructure;
using PartyGames.Web.Models.Bingo;
using WebGrease.Css.Extensions;

namespace PartyGames.Web.Controllers
{
    [RoutePrefix("api/bingo")]
    public class BingoApiController : BaseController
    {
        private readonly IBingoService _bingoService;
        private readonly IPlayerService _playerService;

        public BingoApiController(IBingoService bingoService,
            IPlayerService playerService)
        {
            _bingoService = bingoService;
            _playerService = playerService;
        }

        [HttpPost]
        [Route("GetPlayer")]
        public ActionResult GetPlayer(string key)
        {
            if (string.IsNullOrEmpty(key))
                return AjaxError("Invalid key。");

            try
            {
                var game = _bingoService.GetLiveGame();
                var player = _playerService.GetCachedPlayer(key);


                if (game == null || player == null)
                    throw new Exception("No player found.");

                var playerModel = Mapper.Map(player, new PlayerModel());
                var gameModel = Mapper.Map(game, new GameModel());

                return Json(new
                {
                    Success = true,
                    Player = playerModel,
                    Game = gameModel
                });
            }
            catch (Exception ex)
            {
                return AjaxError(ex.Message);
            }
        }

        [HttpPost]
        [Route("AddPlayer")]
        public ActionResult AddPlayer(string name, string password)
        {
            if (string.IsNullOrEmpty(name))
                return AjaxError("請輸入您的名字。");

            if (string.IsNullOrEmpty(password))
                return AjaxError("請輸入密碼。");

            try
            {
                var game = _bingoService.GetLiveGame();

                if (password != game.Password)
                    return AjaxError("請輸入正確的密碼。");

                var player = _playerService.AddPlayerToGame(game, name);
                var playerModel = Mapper.Map(player, new PlayerModel());
                var gameModel = Mapper.Map(game, new GameModel());

                return Json(new
                {
                    Success = true,
                    Player = playerModel,
                    Game = gameModel
                });
            }
            catch (Exception ex)
            {
                return AjaxError(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetPlayerByCard")]
        public ActionResult GetPlayerByCard(string cardNo, string checkDigit)
        {
            if (string.IsNullOrEmpty(cardNo))
                return AjaxError("請輸入卡號。");

            if (string.IsNullOrEmpty(checkDigit))
                return AjaxError("請輸入卡上的第一個號碼。");

            try
            {
                var game = _bingoService.GetLiveGame();
                var player = _playerService.GetPlayerByCard(game, cardNo, checkDigit);

                if (player == null)
                    return AjaxError("請輸入正確的卡號。");

                var playerModel = Mapper.Map(player, new PlayerModel());
                var gameModel = Mapper.Map(game, new GameModel());

                return Json(new
                {
                    Success = true,
                    Player = playerModel,
                    Game = gameModel
                });
            }
            catch (Exception ex)
            {
                return AjaxError(ex.Message);
            }
        }

        [HttpPost]
        [Route("live-data")]
        public ActionResult GetLiveData(string key)
        {
            if (key != "dashboard")
            {
                var player = _playerService.GetCachedPlayer(key);
                if (player == null)
                    return AjaxError("No player found.");
            }

            var numbers = _bingoService.GetCachedLiveGameRolledNumbers();
            return Json(new
            {
                Success = true,
                Numbers = numbers
            });
        }

        [HttpPost]
        [Route("player-list")]
        public ActionResult GetPlayerList()
        {
            var game = _bingoService.GetLiveGame();
            var players = _playerService.GetPlayersByGame(game).ToModels(game).SortByMarks();

            var rankingList = new List<RankingPlayerModel>();
            var championList = new List<List<RankingPlayerModel>>
            {
                new List<RankingPlayerModel>(),
                new List<RankingPlayerModel>()
            };

            //game not start, get latest player list
            if (game.GetRolledNumberList().Count == 0)
            {
                var ranking = players.Count;

                for (var i = 0; i < players.Count; i++)
                {
                    var player = players[i];

                    rankingList.Add(new RankingPlayerModel
                    {
                        Ranking = ranking,
                        CardNo = player.CardUniqueNo,
                        Name = player.Name,
                        Key = player.Key,
                        BingoMark1 = player.GameMark.Mark1,
                        BingoMark2 = player.GameMark.Mark2,
                        BingoMark3 = 0,
                        Active = false
                    });

                    ranking--;
                }
            }
            else
            {
                foreach (var player in players)
                {
                    Action addToRankingList = () =>
                    {
                        var lastPlayer = rankingList.Skip(Math.Max(0, rankingList.Count - 1)).FirstOrDefault();
                        var ranking = rankingList.Count + 1;

                        //check is same as last player
                        if (lastPlayer != null
                            && lastPlayer.BingoMark1 == player.GameMark.Mark1
                            && lastPlayer.BingoMark2 == player.GameMark.Mark2)
                            ranking = lastPlayer.Ranking;

                        rankingList.Add(new RankingPlayerModel
                        {
                            Ranking = ranking,
                            CardNo = player.CardUniqueNo,
                            Name = player.Name,
                            Key = player.Key,
                            BingoMark1 = player.GameMark.Mark1,
                            BingoMark2 = player.GameMark.Mark2,
                            BingoMark3 = player.GameMark.Mark3.LastOrDefault(),
                            Active = false
                        });
                    };
                    Action<int> addToChamptionList = listIdx =>
                    {
                        if (listIdx != 0 && listIdx != 1)
                            throw new ArgumentException();

                        championList[listIdx].Insert(0, new RankingPlayerModel
                        {
                            Ranking = 0,
                            CardNo = player.CardUniqueNo,
                            Name = player.Name,
                            Key = player.Key,
                            BingoMark1 = player.GameMark.Mark1,
                            BingoMark2 = player.GameMark.Mark2,
                            BingoMark3 = player.GameMark.Mark3[listIdx],
                            Active = false
                        });
                    };

                    var isBingo = player.GameMark.Mark3.Any();

                    if (isBingo)
                    {
                        var runTonyRule = true;
                        //1-5 個要中 1 行
                        //6 個或以上要中 2 行
                        var rulesChangeBreakPoint = runTonyRule ? 5 : 999;
                        var championListIdx = championList[0].Count >= rulesChangeBreakPoint ? 1 : 0;

                        //handle 第5個 slot 有多人中
                        if (championListIdx == 1 && !championList[1].Any())
                        {
                            var lastChampion = championList[0].FirstOrDefault();
                            if (lastChampion != null && player.GameMark.Mark3.First() == lastChampion.BingoMark3)
                                championListIdx = 0;
                        }

                        //*** 6 個或以上要中 2 行 ***
                        if (championListIdx == 0 || championListIdx == 1 && player.GameMark.Mark3.Count >= 2)
                            addToChamptionList(championListIdx);
                        else
                            addToRankingList();
                    }
                    else
                    {
                        addToRankingList();
                    }
                }

                //sort champion list
                if (championList[0].Count > 0)
                    championList[0] = championList[0].OrderByDescending(c => c.BingoMark3).ToList();
                if (championList[1].Count > 0)
                    championList[1] = championList[1].OrderByDescending(c => c.BingoMark3).ToList();

                //set champion ranking, must put after sorting
                //var championRanking = championList.SelectMany(c => c).Count();
                //foreach (var list in championList)
                //{
                //    foreach (var champion in championList[0])
                //    {
                //        champion.Ranking = championRanking;
                //        championRanking--;
                //    }
                //}

                if (championList[0].Count > 0)
                {
                    var championRanking = championList[0].Count;
                    foreach (var champion in championList[0])
                    {
                        champion.Ranking = championRanking;
                        championRanking--;
                    }
                }
                if (championList[1].Count > 0)
                {
                    var championRanking = championList[0].Count + championList[1].Count;
                    foreach (var champion in championList[1])
                    {
                        champion.Ranking = championRanking;
                        championRanking--;
                    }
                }


                //active the first player

                var activeChampionList = championList[1].Count == 0 ? championList[0] : championList[1];
                activeChampionList.Where(c => c.BingoMark3 == game.GetRolledNumberList().Count).ForEach(c => c.Active = true);

                rankingList.Where(c => c.Ranking == rankingList.Min(m => m.Ranking)).ForEach(c => c.Active = true);
            }

            return Json(new
            {
                Success = true,
                RankingList = rankingList,
                ChampionList = championList[1].Concat(championList[0])
            });
        }

        [HttpPost]
        [Route("roll")]
        public ActionResult BingoRoll()
        {
            Thread.Sleep(5000);

            var game = _bingoService.GetLiveGame();
            var newNumber = _bingoService.RollNewNumber(game);

            var gameModel = Mapper.Map(game, new GameModel());

            return Json(new
            {
                Game = gameModel,
                NewNumber = newNumber,
                D1 = newNumber.ToString("00").Substring(0, 1),
                D2 = newNumber.ToString("00").Substring(1, 1)
            });
        }

        [HttpPost]
        [Route("change-name")]
        public ActionResult ChangeName(string name, string key)
        {
            var player = _playerService.GetPlayerByKey(key);

            if (player == null || string.IsNullOrEmpty(name))
                return AjaxError("Invalid player.");

            player.Name = name.Trim();
            _playerService.UpdatePlayer(player);


            return Json(new
            {
                Success = true,
                Player = Mapper.Map(player, new PlayerModel())
            });
        }
    }
}