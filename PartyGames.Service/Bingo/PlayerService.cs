using System;
using System.Collections.Generic;
using System.Linq;
using PartyGames.Data;
using PartyGames.Data.BingoContext;
using PartyGames.Service.Caching;

namespace PartyGames.Service.Bingo
{
    public class PlayerService : IPlayerService
    {

        private readonly ICacheManager _cacheManager;
        private readonly IRepository<Card> _cardRepository;
        private readonly IRepository<Player> _playerRepository;
        private readonly IBingoService _bingoService;

        public PlayerService(IRepository<Card> cardRepository,
            IRepository<Player> playerRepository,
            ICacheManager cacheManager, IBingoService bingoService)
        {
            _cardRepository = cardRepository;
            _playerRepository = playerRepository;
            _cacheManager = cacheManager;
            _bingoService = bingoService;
        }

        public void AddTestingPlayer(Game game, int count)
        {
            for (var i = 0; i < count; i++)
                AddPlayerToGame(game, "Player " + (i + 1));
        }

        public Player AddPlayerToGame(Game game, string name, Card card = null)
        {
            if (card == null)
                card = _bingoService.GetNewUniqueCard();

            var player = new Player
            {
                GameId = game.GameId,
                Key = Guid.NewGuid().ToString("N"),
                Name = name,
                CardUniqueNo = card.CardNo,
                CardNumbers = card.Numbers,
                LastUpdate = DateTime.Now,
                CreateDate = DateTime.Now
            };

            _playerRepository.Add(player);
            return player;
        }

        public void UpdatePlayer(Player player)
        {
            _playerRepository.Update(player);

            var cacheKey = string.Format(CacheKey.BingoPlayer, player.Key);
            _cacheManager.Remove(cacheKey);
        }

        public void DeletePlayer(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;

            var player = GetPlayerByKey(key);

            if (player != null)
                _playerRepository.Delete(player);

            var cacheKey = string.Format(CacheKey.BingoPlayer, key);
            _cacheManager.Remove(cacheKey);
        }

        public Player GetPlayerByKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            return _playerRepository.Table.FirstOrDefault(c => c.Key == key);
        }

        public Player GetCachedPlayer(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            var cacheKey = string.Format(CacheKey.BingoPlayer, key);
            return _cacheManager.Get(cacheKey, () => GetPlayerByKey(key));
        }

        public IList<Player> GetPlayersByGame(Game game)
        {
            return _playerRepository.Table.Where(c => c.GameId == game.GameId).ToList();
        }

        public Player AssignPlayerToCard(Game game, string name, Card card)
        {
            card.Used = true;
            _cardRepository.Update(card);

            return AddPlayerToGame(game, name, card);
        }

    }
}
