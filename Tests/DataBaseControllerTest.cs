using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using NUnit.Framework;
using Kontur.GameStats.Server;

namespace Tests
{
    [TestFixture]
    public class DataBaseControllerTest
    {
        SimpleDataBase dataBase;
        DataBaseController controller;

        [SetUp]
        public void SetUp()
        {

        }

        [OneTimeSetUp]
        public void TestInitialize()
        {
            dataBase = new SimpleDataBase();
            controller = new DataBaseController(dataBase);

            dataBase.AddServer(new Server("test-8080", "test server", "DM", "TDM"));
            dataBase.AddServer(new Server("167.42.23.32-1337", "] My P3rfect Server [", "DM"));

            var scoreBoard = new List<ScoreBoardUnit>()
            {
                new ScoreBoardUnit("Player1", 20, 21, 3),
                new ScoreBoardUnit("Player2", 2, 2, 21)
            };
            dataBase.PutMatch("test-8080", "2017-01-22T15:17:00Z", 
                new MatchInfo("DM-HelloWorld", "DM", 20, 20, 12.345678, scoreBoard));
        }

        [Test]
        public void Put_Server_TestRequest()
        {
            var commandParameter = "/servers/myserver-8080/info";
            var newServer = new ServerInfo("my server", "my server", "DM", "TDM");
            var dataStream = new MemoryStream();
            Serializer.SerializeObject(newServer, dataStream);

            controller.HandleRequest(MethodType.PUT, commandParameter, dataStream);

            Assert.Contains(newServer, dataBase.GetAllServers().Select(s => s.info).ToArray());
        }

        [Test]
        public void Get_ServerInfo_TestRequest()
        {
            var commandParameter = "/servers/test-8080/info";
            var server = dataBase.GetServer("test-8080");

            var answer = controller.HandleRequest(MethodType.GET, commandParameter);
            var answerStatus = answer.Item1;
            var answerData = (ServerInfo)Serializer.DeserializeObject(typeof(ServerInfo), answer.Item2);

            Assert.AreEqual(HttpStatusCode.OK, answerStatus);
            Assert.AreEqual(server.info, answerData);
        }

        [Test]
        public void Put_Match_TestRequest()
        {
            var time = "2000-02-20T20:20:20Z";
            var commandParameter = $"/servers/test-8080/matches/{time}";
            var scoreBoard = new List<ScoreBoardUnit>()
            {
                new ScoreBoardUnit("Player1", 20, 21, 3),
                new ScoreBoardUnit("Player2", 2, 2, 21)
            };
            var newMatch = new MatchInfo("DM-HelloWorld", "DM", 20, 20, 12.345678, scoreBoard);
            var dataStream = new MemoryStream();
            Serializer.SerializeObject(newMatch, dataStream);

            controller.HandleRequest(MethodType.PUT, commandParameter, dataStream);

            Assert.AreEqual(newMatch, dataBase.GetMatch("test-8080", time));
        }

        [Test]
        public void Get_Match_TestRequest()
        {
            var time = "2017-01-22T15:17:00Z";
            var commandParameter = $"/servers/test-8080/matches/{time}";
            var match = dataBase.GetMatch("test-8080", time);

            var answer = controller.HandleRequest(MethodType.GET, commandParameter);
            var answerData = (MatchInfo)Serializer.DeserializeObject(typeof(MatchInfo), answer.Item2);
            var answerStatus = answer.Item1;

            Assert.AreEqual(match, answerData);
            Assert.AreEqual(HttpStatusCode.OK, answerStatus);
        }

        [Test]
        public void Get_AllServersInfo_RequestTest()
        {
            var commandParameters = "/servers/info";

            var answer = controller.HandleRequest(MethodType.GET, commandParameters);
            var servers = (List<Server>) Serializer.DeserializeObject(typeof(List<Server>), answer.Item2);

            Assert.True(dataBase
                .GetAllServers()
                .Select(s => s.info)
                .SequenceEqual(servers.Select(s => s.info)));
            ;
        }

        // Tests for the correctness of statistics in a separate file
        [Test]
        public void Get_ServerStats_RequestTest()
        {
            var commandParameters = "/servers/test-8080/stats";
            var stats = dataBase.GetServerStats("test-8080");

            var answer = controller.HandleRequest(MethodType.GET, commandParameters);
            var answerStats = (ServerStats) Serializer.DeserializeObject(typeof(ServerStats), answer.Item2);

            Assert.AreEqual(stats, answerStats);
        }

        [Test]
        public void Get_PlayersStats_RequestTest()
        {
            var commandParameters = "/players/Player1/stats";
            var stats = dataBase.GetPlayerStats("Player1");

            var answer = controller.HandleRequest(MethodType.GET, commandParameters);
            var answerStats = (PlayerStats)Serializer.DeserializeObject(typeof(PlayerStats), answer.Item2);

            Assert.AreEqual(stats, answerStats);
        }

        [Test]
        public void Get_RecentMatches_RequestTest()
        {
            var commandParameters = "/reports/recent-matches";
            var recentMatches = dataBase.GetRecentMatches(5);

            var answer = controller.HandleRequest(MethodType.GET, commandParameters);
            var result = (List<RecentMatchInfo>) Serializer.DeserializeObject(typeof(List<RecentMatchInfo>), answer.Item2);

            Assert.True(recentMatches.SequenceEqual(result));
        }

        [Test]
        public void Get_BestPlayers_RequestTest()
        {
            var commandParameters = "/reports/best-players";
            var bestPlayers = dataBase.GetBestPlayers();

            var answer = controller.HandleRequest(MethodType.GET, commandParameters);
            var result = (List<BestPlayerInfo>) Serializer.DeserializeObject(typeof(List<BestPlayerInfo>), answer.Item2);

            Assert.True(bestPlayers.SequenceEqual(result));
        }

        [Test]
        public void Get_PopularServers_RequestTest()
        {
            var commandParameters = "/reports/popular-servers";
            var popularServers = dataBase.GetPopularServers();

            var answer = controller.HandleRequest(MethodType.GET, commandParameters);
            var result = (List<PopularServerInfo>)Serializer.DeserializeObject(typeof(List<PopularServerInfo>), answer.Item2);

            Assert.True(popularServers.SequenceEqual(result));
        }
    }
}
