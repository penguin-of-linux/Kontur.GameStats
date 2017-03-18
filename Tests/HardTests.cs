using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kontur.GameStats.Server;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class HardTests
    {
        private DataBase dataBase;
        private DataBaseController controller;
        private Random rnd;
        private string[] maps;
        private string[] gameModes;
        private string[] players;

        [OneTimeSetUp]
        public void TestInitialize()
        {
            rnd = new Random();
            dataBase = new DataBase();
            controller = new DataBaseController(dataBase);

            gameModes = new string[] {"DM", "TDM", "CTF", "SG", "BTL"};
            maps = new string[] { "Isengard", "Barad-Dur", "Minas-Tirith", "Edoras", "Umbar", "Dale", "Peralrgir", "Rivendel" };
            players = new string[1000 * 1000];
            Enumerable.Range(0, 1000 * 1000 - 1)
                      .AsParallel()
                      .ForAll(i => players[i] = $"player{i}");

            PuttingResults();
        }

        //[Test]
        public void PuttingResults()
        {
            //dataBase.AddServer(new Server("test-8080", "test server", "DM", "TDM"));
            //dataBase.AddServer(new Server("test2-8080", "] My P3rfect Server [", "DM"));

            /*var scoreBoard = new List<ScoreBoardUnit>()
            {
                new ScoreBoardUnit("Player1", 20, 21, 3),
                new ScoreBoardUnit("Player2", 2, 2, 21)
            };
            dataBase.PutMatch("test-8080", "2017-01-22T15:17:00Z",
                new MatchInfo("DM-HelloWorld", "DM", 20, 20, 12.345678, scoreBoard));*/

            /*var server = dataBase.GetServer("test2-8080");
            for (int i = 0; i < 10000; i++)
                dataBase.PutMatch("test2-8080", $"2017-01-{(i % 26) + 1}T{i % 12 + 10}:{i % 41 + 10}:{i % 50 + 10}Z",
                    new MatchInfo("DM-HelloWorld", "DM", 20, 20, 12.345678, scoreBoard));*/
            

            /*for (int s = 0; s < 1 * 10; s++)
            {
                var server = new Server($"test-{s}", $"name of test-{s} server", gameModes);
                dataBase.AddServer(server);

                for (int d = 1; d < 15; d++)
                {
                    //var time = $"2017-01-{d}T15:17:00Z";

                    for (int m = 0; m < 1; m++)
                    {
                        var scoreBoard = GetRandomScoreBoad();
                        var match = new MatchInfo(maps[rnd.Next(0, 7)],
                                                  gameModes[rnd.Next(0, 4)],
                                                  1000,
                                                  1000,
                                                  1000.0,
                                                  scoreBoard);
                        dataBase.PutMatch(server.Endpoint, $"2017-01-{d}T{m % 14 + 10}:{m % 50 + 10}:00Z", match);
                    }
                }
            }*/
        }

        [Test]
        public void DataBaseOverburdened_Test()
        {
            //var serversCount = dataBase.GetAllServers().Count;
            var matchesCount = dataBase.GetAllMatches().Count;
            Assert.Fail();
        }

        private List<ScoreBoardUnit> GetRandomScoreBoad()
        {
            var result = new List<ScoreBoardUnit>();
            var startPos = rnd.Next(0, players.Length - 101);
            for (int i = startPos; i < startPos + 100; i++)
            {
                result.Add(new ScoreBoardUnit(players[i], startPos + 101 - i, startPos + 101 - i, i - startPos + 1));
            }

            return result;
        }
    }
}
