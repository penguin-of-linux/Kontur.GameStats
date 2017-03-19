using System.Collections.Generic;
using System.Linq;
using Kontur.GameStats.Server;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class ReportsCommandTests
    {
        SimpleDataBase dataBase;
        DataBaseController controller;

        [OneTimeSetUp]
        public void TestInitialize()
        {

        }

        [SetUp]
        public void SetUp()
        {
            dataBase = new SimpleDataBase();
            controller = new DataBaseController(dataBase);
        }

        [Test]
        public void Get_RecentMatches_Test()
        {
            var scoreBoard = new List<ScoreBoardUnit>
            {
                new ScoreBoardUnit("Player1", 20, 21, 3),
                new ScoreBoardUnit("Player2", 2, 2, 21)
            };
            dataBase.AddServer(new Server("test1-8080", "test server", "DM"));
            dataBase.AddServer(new Server("test2-8080", "test server", "DM"));
            var match = new MatchInfo("DM-HelloWorld", "DM", 20, 20, 12.345678, scoreBoard);


            for (int i = 16; i < 21; i++)
            {
                dataBase.PutMatch("test1-8080", $"2017-01-22T{i}:17:00Z", match);
            }

            for (int i = 10; i < 15; i++)
            {
                dataBase.PutMatch("test2-8080", $"2017-01-22T{i}:17:00Z", match);
            }

            var result = dataBase.GetRecentMatches();

            Assert.True(dataBase.GetAllServers()
                                .Single(s => s.endpoint == "test1-8080")
                                .Matches.Values
                                .SequenceEqual(result.Select(x => x.results)));
        }

        [Test]
        public void Get_BestPlayers()
        {
            var scoreBoard1 = new List<ScoreBoardUnit>
            {
                new ScoreBoardUnit("Player1", 10, 21, 3),
                new ScoreBoardUnit("Player2", 10, 1, 1),
                new ScoreBoardUnit("Player3", 10, 1, 1),
                new ScoreBoardUnit("Player4", 10, 1, 1)
            };

            var scoreBoard2 = new List<ScoreBoardUnit>
            {
                new ScoreBoardUnit("Player4", 10, 199, 1),
                new ScoreBoardUnit("Player1", 10, 1, 1)
            };

            dataBase.AddServer(new Server("test1-8080", "test server", "DM"));
            dataBase.AddServer(new Server("test2-8080", "test server", "DM"));

            var match1 = new MatchInfo("DM-HelloWorld", "DM", 20, 20, 12.345678, scoreBoard1);
            var match2 = new MatchInfo("DM-HelloWorld", "DM", 20, 20, 12.345678, scoreBoard2);

            dataBase.PutMatch("test1-8080", "2017-01-22T15:17:00Z", match1);
            dataBase.PutMatch("test1-8080", "2017-01-22T15:17:00Z", match2);

            var result = dataBase.GetBestPlayers().Select(bpi => bpi.name);

            Assert.True(result.SequenceEqual(new [] {"player4", "player1", "player2", "player3"}));
        }

        [Test]
        public void Get_PopularServers_Test()
        {
            var emptyMatch = new MatchInfo("1", "1", 1, 1, 1, new List<ScoreBoardUnit>());

            dataBase.AddServer(new Server("1-8080", "1", "DM"));
            dataBase.AddServer(new Server("2-8080", "2", "DM"));
            dataBase.AddServer(new Server("3-8080", "3", "DM"));

            // 1-8080: 0 matches per day
            // 2-8080: 2 matches per day
            // 3-8080: 1 matches per day

            dataBase.PutMatch("2-8080", "2017-01-01T10:17:00Z", emptyMatch);
            dataBase.PutMatch("2-8080", "2017-01-01T10:17:00Z", emptyMatch);
            dataBase.PutMatch("2-8080", "2017-01-30T10:17:00Z", emptyMatch);
            dataBase.PutMatch("2-8080", "2017-01-30T10:17:00Z", emptyMatch);

            dataBase.PutMatch("3-8080", "2017-01-01T15:17:00Z", emptyMatch);
            dataBase.PutMatch("3-8080", "2017-01-30T15:17:00Z", emptyMatch);

            var result = dataBase.GetPopularServers().Select(psi => psi.name).ToList();

            Assert.True(result.SequenceEqual(new [] {"2", "3", "1"}));
        }
    }
}
