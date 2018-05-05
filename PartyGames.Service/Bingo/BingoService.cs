using System;
using System.Collections.Generic;
using System.Linq;
using PartyGames.Data;
using PartyGames.Data.BingoContext;
using PartyGames.Service.Caching;
using PartyGames.Service.WebService;

namespace PartyGames.Service.Bingo
{
    public class BingoService : IBingoService
    {
        private static readonly object o_uniqueCardLock = new object();
        private static readonly object o_roll_lock = new object();

        private readonly ICacheManager _cacheManager;
        private readonly IRepository<Card> _cardRepository;
        private readonly IRepository<Game> _gameRepository;
        private readonly IEposWebService _webService;

        public BingoService(IRepository<Card> cardRepository,
            IRepository<Game> gameRepository,
            ICacheManager cacheManager,
            IEposWebService webService)
        {
            _cardRepository = cardRepository;
            _gameRepository = gameRepository;
            _cacheManager = cacheManager;
            _webService = webService;
        }

        public List<Card> GetCards()
        {
            return _cardRepository.Table.ToList();
        }

        public void ResetBingo(bool resetCard)
        {
            //reset game
            var game = GetLiveGame();
            game.RolledNumbers = null;
            _gameRepository.Update(game);

            _cardRepository.ExecuteSqlCommand("DELETE FROM [Player]");
            _cardRepository.ExecuteSqlCommand("ALTER TABLE [Player] ALTER COLUMN [PlayerId] IDENTITY (1,1)");

            if (resetCard)
            {
                //Delete card in db
                _cardRepository.ExecuteSqlCommand("DELETE FROM [Card]");
                _cardRepository.ExecuteSqlCommand("ALTER TABLE [Card] ALTER COLUMN [CardId] IDENTITY (1,1)");

                var cards = _webService.GetBingoCards();
                var cardList = cards.Data.Select(c => new Card
                {
                    CardNo = c.CardNo,
                    Numbers = c.Numbers,
                    CardType = c.CardType,
                    Used = false
                });

                _cardRepository.Add(cardList);
            }
            else
            {
                _cardRepository.ExecuteSqlCommand("UPDATE [Card] SET Used = 0");
            }

            _cacheManager.RemoveByPattern(CacheKey.BingoGamePattern);
            _cacheManager.RemoveByPattern(CacheKey.BingoPlayerPattern);
        }

        public List<int> GetCachedLiveGameRolledNumbers()
        {
            return _cacheManager.Get(CacheKey.BingoGameStatus, () =>
            {
                var game = GetLiveGame();
                return game.GetRolledNumberList();
            });
        }

        public Game GetLiveGame()
        {
            return GetGameByCode(null);
        }

        public Game GetGameByCode(string code)
        {
            var query = _gameRepository.Table;

            query = string.IsNullOrEmpty(code)
                ? query.OrderByDescending(c => c.CreateDate).Take(1)
                : query.Where(c => c.Code == code);

            return query.FirstOrDefault();
        }

        public Card GetCardByCardNo(string cardNo)
        {
            if (string.IsNullOrEmpty(cardNo))
                return null;

            return _cardRepository.Table.FirstOrDefault(c => c.CardNo == cardNo);
        }

        public int RollNewNumber(Game game)
        {
            lock (o_roll_lock)
            {
                var numberList = game.GetRolledNumberList();
                var random = new Random();

                //get a new number (1-75)
                var newNumber = random.Next(1, 76);

                //random again if number exists
                while (numberList.Contains(newNumber))
                    newNumber = random.Next(1, 76);

                numberList.Add(newNumber);
                game.RolledNumbers = string.Join(",", numberList);

                //save game
                _gameRepository.Update(game);

                //remove cache
                _cacheManager.RemoveByPattern(CacheKey.BingoGamePattern);
                return newNumber;
            }
        }

        public Card GetNewUniqueCard()
        {
            lock (o_uniqueCardLock)
            {
                var card = _cardRepository.Table.Where(c => !c.Used && c.CardType == "RANDOM").Take(1).First();

                card.Used = true;
                _cardRepository.Update(card);

                return card;
            }
        }


    }
}