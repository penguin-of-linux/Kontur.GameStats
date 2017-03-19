using System.Collections.Generic;
using System.Linq;
using Kontur.GameStats.Server;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class StatsTest
    {
        private SimpleDataBase dataBase;
        private DataBaseController controller;

        [OneTimeSetUp]
        public void TestInitialize()
        {
            dataBase = new SimpleDataBase();
            controller = new DataBaseController(dataBase);

            dataBase.AddServer(new Server("test-8080", "test server", "DM", "TDM"));
            dataBase.AddServer(new Server("test2-8080", "] My P3rfect Server [", "DM"));

            var scoreBoard = new List<ScoreBoardUnit>()
            {
                new ScoreBoardUnit("Player1", 20, 21, 3),
                new ScoreBoardUnit("Player2", 2, 2, 21)
            };
            dataBase.PutMatch("test-8080", "2017-01-22T15:17:00Z",
                new MatchInfo("DM-HelloWorld", "DM", 20, 20, 12.345678, scoreBoard));
        }

        [Test]
        public void ServerStats_Simple_RequestTest()
        {
            var parameters = "/servers/test-8080/stats";

            var answer = controller.HandleRequest(MethodType.GET, parameters);
            var stats = (ServerStats) Serializer.DeserializeObject(typeof(ServerStats), answer.Item2);

            Assert.AreEqual(1, stats.totalMatchesPlayed, "TotalMatchesPlayed");
            Assert.AreEqual(1, stats.maximumMatchesPerDay, "MaximumMatchesPerDay");
            Assert.AreEqual(1, stats.averageMatchesPerDay, "AverageMatchesPerDay");
            Assert.AreEqual(2, stats.maximumPopulation, "MaximumPopulation");
            Assert.AreEqual(2, stats.averagePopulation, "AveragePopulation");
            Assert.True(new [] {"DM"}.SequenceEqual(stats.top5GameModes), "Top5GameModes");
            Assert.True(new [] {"DM-HelloWorld"}.SequenceEqual(stats.top5Maps), "Top5Maps");
        }

        [Test]
        public void PlayerStats_Simple_Test()
        {
            var name = "Player1";

            var stats = dataBase.GetPlayerStats(name);

            Assert.AreEqual(1, stats.averageMatchesPerDay);
            Assert.AreEqual(100, stats.averageScoreboardPercent);
            Assert.AreEqual("DM", stats.favoriteGameMode);
            Assert.AreEqual("test-8080", stats.favoriteServer);
            Assert.AreEqual((double) 21 / 3, stats.killToDeathRatio);
            Assert.AreEqual("2017-01-22T15:17:00Z", stats.lastMatchPlayed);
            Assert.AreEqual(1, stats.maximumMatchesPerDay);
            Assert.AreEqual(1, stats.totalMatchesPlayed);
            Assert.AreEqual(1, stats.totalMatchesWon);
            Assert.AreEqual(1, stats.uniqueServers);
        }
    }
}
