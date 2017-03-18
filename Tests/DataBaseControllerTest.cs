using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Linq;
using System.Text;
using NUnit;
using NUnit.Framework;
using Kontur.GameStats.Server;

namespace Tests
{
    [TestFixture]
    public class DataBaseControllerTest
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

            dataBase.AddServer(new Server("test-8080", "test server", "DM", "TDM"));
            dataBase.AddServer(new Server("167.42.23.32-1337", "] My P3rfect Server [", "DM"));

            var scoreBoard = new List<ScoreBoardUnit>()
            {
                new ScoreBoardUnit("Player1", 20, 21, 3),
                new ScoreBoardUnit("Player2", 2, 2, 21)
            };
            dataBase.PutMatch("test-8080", "2017 -01-22T15:17:00Z", 
                new MatchInfo("DM-HelloWorld", "DM", 20, 20, 12.345678, scoreBoard));
        }

        [Test]
        public void DataBaseIsSafe_Test()
        {
            var serverCopy = dataBase.GetServer("test-8080");
            var matchCopy = dataBase.GetMatch("test-8080", "2017 -01-22T15:17:00Z");
            var statsCopy = dataBase.GetServerStats("test-8080");

            serverCopy = new Server("1", "2", "3");
            matchCopy = new MatchInfo("1", "2", 3, 4, 5.0, null);
            statsCopy = new ServerStats();

            Assert.Fail();
            Assert.AreNotEqual(dataBase.GetServer("test-8080"), serverCopy);
            Assert.AreNotEqual(dataBase.GetMatch("test-8080", "2017 -01-22T15:17:00Z"), matchCopy);
            Assert.AreNotEqual(dataBase.GetServerStats("test-8080"), statsCopy);
        }

        [Test]
        public void Put_Server_TestRequest()
        {
            var commandParameter = "/servers/myserver-8080/info";
            var newServer = new ServerInfo("my server", "my server", "DM", "TDM");
            var dataStream = new MemoryStream();
            Serializer.SerializeObject(newServer, dataStream);

            controller.HandleRequest(MethodType.Put, commandParameter, dataStream);

            Assert.Contains(newServer, dataBase.GetAllServers().Select(s => s.Info).ToArray());
        }

        [Test]
        public void Get_ServerInfo_TestRequest()
        {
            var commandParameter = "/servers/test-8080/info";
            var server = dataBase.GetServer("test-8080");

            var answer = controller.HandleRequest(MethodType.Get, commandParameter);
            var answerStatus = answer.Item1;
            var answerData = (ServerInfo)Serializer.DeserializeObject(typeof(ServerInfo), answer.Item2);

            Assert.AreEqual(StatusCode.OK, answerStatus);
            Assert.AreEqual(server.Info, answerData);
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

            controller.HandleRequest(MethodType.Put, commandParameter, dataStream);

            Assert.AreEqual(newMatch, dataBase.GetMatch("test-8080", time));
        }

        [Test]
        public void Get_Match_TestRequest()
        {
            var time = "2017-01-22T15:17:00Z";
            var commandParameter = $"/servers/test-8080/matches/{time}";
            var match = dataBase.GetMatch("test-8080", time);

            var answer = controller.HandleRequest(MethodType.Get, commandParameter);
            var answerData = (MatchInfo)Serializer.DeserializeObject(typeof(MatchInfo), answer.Item2);
            var answerStatus = answer.Item1;

            Assert.AreEqual(match, answerData);
            Assert.AreEqual(StatusCode.OK, answerStatus);
        }

        [Test]
        public void Get_AllServersInfo_RequestTest()
        {
            var commandParameters = "/servers/info";

            var answer = controller.HandleRequest(MethodType.Get, commandParameters);
            var servers = (List<Server>) Serializer.DeserializeObject(typeof(List<Server>), answer.Item2);

            Assert.True(dataBase
                .GetAllServers()
                .Select(s => s.Info)
                .SequenceEqual(servers.Select(s => s.Info)));
            ;
        }

        // Tests for the correctness of statistics in a separate file
        [Test]
        public void Get_ServerStats_RequestTest()
        {
            var commandParameters = "/servers/test-8080/stats";
            var stats = dataBase.GetServerStats("test-8080");

            var answer = controller.HandleRequest(MethodType.Get, commandParameters);
            var answerStats = (ServerStats) Serializer.DeserializeObject(typeof(ServerStats), answer.Item2);

            Assert.AreEqual(stats, answerStats);
        }

        [Test]
        public void Get_PlayersStats_RequestTest()
        {
            var commandParameters = "/players/Player1/stats";
            var stats = dataBase.GetPlayerStats("Player1");

            var answer = controller.HandleRequest(MethodType.Get, commandParameters);
            var answerStats = (PlayerStats)Serializer.DeserializeObject(typeof(PlayerStats), answer.Item2);

            Assert.AreEqual(stats, answerStats);
        }

        [Test]
        public void Get_RecentMatches_RequestTest()
        {
            var commandParameters = "/reports/recent-matches";
            var recentMatches = dataBase.GetRecentMatches(5);

            var answer = controller.HandleRequest(MethodType.Get, commandParameters);
            var result = (List<RecentMatchInfo>) Serializer.DeserializeObject(typeof(List<RecentMatchInfo>), answer.Item2);

            Assert.True(recentMatches.SequenceEqual(result));
        }

        [Test]
        public void Get_BestPlayers_RequestTest()
        {
            var commandParameters = "/reports/best-players";
            var bestPlayers = dataBase.GetBestPlayers();

            var answer = controller.HandleRequest(MethodType.Get, commandParameters);
            var result = (List<BestPlayerInfo>) Serializer.DeserializeObject(typeof(List<BestPlayerInfo>), answer.Item2);

            Assert.True(bestPlayers.SequenceEqual(result));
        }
    }
}
