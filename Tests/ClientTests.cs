﻿using System;
using System.IO;
using System.Net;
using Kontur.GameStats.Server;
using NUnit.Framework;

namespace Tests
{
    [TestFixture()]
    public class ClientTests
    {
        StatServer server = new StatServer();

        [SetUp]
        public void SetUp()
        {
            server.Start("http://+:8080/");
        }

        [TearDown]
        public void TearDown()
        {
            server.Stop();
        }

        [Test]
        public void AddServer_ClientTest()
        {
            var data = "{\"name\": \"1\",\"gameModes\": [ \"DM\", \"TDM\" ]}";
            var parameters = "/servers/1-8080/info";

            var result = GetResponse(data, parameters, MethodType.PUT);
            
            Assert.AreEqual(HttpStatusCode.OK, result.Item1);
            Assert.AreEqual("Answer: ", result.Item2.Replace("\r\n", ""));
        }

        [Test]
        public void Get_ServersInfo_ClientTest()
        {
            AddServer_ClientTest();

            var data = "";
            var parameters = "/servers/info";

            var result = GetResponse(data, parameters, MethodType.GET);

            Assert.AreEqual(HttpStatusCode.OK, result.Item1);
            Assert.AreEqual("Answer: [{\"endpoint\":\"1-8080\",\"info\":{\"gameModes\":[\"DM\",\"TDM\"],\"name\":\"1\"}}]",
                result.Item2.Replace("\r\n", ""));
        }

        [Test]
        public void Get_BestPlayers_EmptyList_ClientTest()
        {
            var data = "";
            var parameters = "/reports/best-players";

            var result = GetResponse(data, parameters, MethodType.GET);

            Assert.AreEqual(HttpStatusCode.OK, result.Item1);
            Assert.AreEqual("Answer: []", result.Item2.Replace("\r\n", ""));
        }

        [Test]
        public void Get_PopularServers_ClientTest()
        {
            AddServer_ClientTest();

            var data = "";
            var parameters = "/reports/popular-servers";

            var result = GetResponse(data, parameters, MethodType.GET);

            Assert.AreEqual(HttpStatusCode.OK, result.Item1);
            Assert.AreEqual("Answer: [{\"averageMatchesPerDay\":0,\"endpoint\":\"1-8080\",\"name\":\"1\"}]",
                result.Item2.Replace("\r\n", ""));
        }

        [Test]
        public void Put_MatchToUnexistingServer_ClientTest()
        {
            var data = "same data";
            var parameters = "/servers/unexisting-server/matches/2017-01-22T15:17:00Z";

            Assert.Throws<WebException>(() => GetResponse(data, parameters, MethodType.GET));
        }

        private Tuple<HttpStatusCode, string> GetResponse(string data, string parameters, MethodType method)
        {
            var request2 = (HttpWebRequest)WebRequest.Create("http://localhost:8080" + parameters);
            request2.Method = method.ToString();

            if (method == MethodType.PUT)
            {
                using (var writer = new StreamWriter(request2.GetRequestStream()))
                {
                    writer.Write(data);
                }
            }

            var result = "";

            var response2 = (HttpWebResponse)request2.GetResponse();
            using (var stream = new StreamReader(response2.GetResponseStream()))
            {
                result = stream.ReadToEnd();
            }
            response2.Close();

            return new Tuple<HttpStatusCode, string>(response2.StatusCode, result);
        }
    }
}
