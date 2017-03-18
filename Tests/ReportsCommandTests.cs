using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kontur.GameStats.Server;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class ReportsCommandTests
    {
        DataBase dataBase;
        DataBaseController controller;

        [SetUp]
        public void SetUp()
        {

        }

        [OneTimeSetUp]
        public void TestInitialize()
        {
            dataBase = new DataBase();
            controller = new DataBaseController(dataBase);

            dataBase.AddServer(new Server("test1-8080", "test server", "DM"));
            dataBase.AddServer(new Server("test2-8080", "test server", "DM"));
        }

        [Test]
        public void Get_RecentMatches_Test()
        {
            var scoreBoard = new List<ScoreBoardUnit>
            {
                new ScoreBoardUnit("Player1", 20, 21, 3),
                new ScoreBoardUnit("Player2", 2, 2, 21)
            };

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
                                .Single(s => s.Endpoint == "test1-8080")
                                .Matches.Values
                                .SequenceEqual(result.Select(x => x.Results)));
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

            var match1 = new MatchInfo("DM-HelloWorld", "DM", 20, 20, 12.345678, scoreBoard1);
            var match2 = new MatchInfo("DM-HelloWorld", "DM", 20, 20, 12.345678, scoreBoard2);

            dataBase.PutMatch("test1-8080", "2017-01-22T15:17:00Z", match1);
            dataBase.PutMatch("test1-8080", "2017-01-22T15:17:00Z", match2);

            var result = dataBase.GetBestPlayers().Select(bpi => bpi.Name);

            Assert.True(result.SequenceEqual(new List<string> {"Player4", "Player1", "Player2", "Player3"}));
        }
    }
}
