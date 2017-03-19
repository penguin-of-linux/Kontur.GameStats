using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Kontur.GameStats.Server.DataTypes.ReportTypes;


namespace Kontur.GameStats.Server
{
    public class DataBase
    {
        private Dictionary<string, Server> servers;
        private Dictionary<string, PlayerStats> playersStats;
        //rivate SortedDictionary<DateTime, MatchInfo> matches;

        public DataBase()
        {
            servers = new Dictionary<string, Server>();
            playersStats = new Dictionary<string, PlayerStats>();
            //matches = new SortedDictionary<DateTime, MatchInfo>(new DescendingComparer<DateTime>());
        }

        public void AddServer(Server server)
        {
            servers[server.endpoint] = server;
        }

        public List<Server> GetAllServers()
        {
            return new List<Server>(servers.Values);
        }

        public List<MatchInfo> GetAllMatches()
        {
            return new List<MatchInfo>(servers.Values.SelectMany(s => s.Matches.Values));
        }

        public Server GetServer(string endpoint)
        {
            return servers[endpoint];
        }

        public void PutMatch(string endpoint, string time, MatchInfo match)
        {
            //var dateTime = DateTime.Parse(time);
            //matches[dateTime] = match;  // !!!!!!!!!!!!!!!!!!!!
            servers[endpoint].Matches[time] = match;
            GetServerStats(endpoint).Update(time, match);

            foreach (var player in match.Players)
            {
                if (!playersStats.ContainsKey(player))
                    playersStats[player] = new PlayerStats(player);

                playersStats[player].Update(match, endpoint, time);
            }
        }

        public MatchInfo GetMatch(string endpoint, string time)
        {
            return servers[endpoint].Matches[time];
        }

        public ServerStats GetServerStats(string endpoint)
        {
            return servers[endpoint].Stats;
        }

        public PlayerStats GetPlayerStats(string name)
        {
            return playersStats[name];
        }

        public List<RecentMatchInfo> GetRecentMatches(int count = 5)
        {
            if (count <= 0) return new List<RecentMatchInfo>();
            if (count > 50) count = 50; // !!!

            return servers.Values
                .SelectMany(s => s.Matches
                    .Select(kvp => new KeyValuePair<DateTime, RecentMatchInfo>(DateTime.Parse(kvp.Key),
                        new RecentMatchInfo
                        {
                            server = s.endpoint,
                            timestamp = kvp.Key,
                            results = kvp.Value
                        })))
                .OrderByDescending(kvp => kvp.Key)
                .Take(count)
                .Select(kvp => kvp.Value)
                .ToList();
        }

        public List<BestPlayerInfo> GetBestPlayers(int count = 5)
        {
            if (count <= 0) return new List<BestPlayerInfo>();
            if (count > 50) count = 50; // !!!

            return playersStats.Values
                               .OrderByDescending(ps => ps.killToDeathRatio)
                               .Take(count)
                               .Select(ps => new BestPlayerInfo { name = ps.Name, killToDeathRatio = ps.killToDeathRatio })
                               .ToList();
        }

        public List<PopularServerInfo> GetPopularServers(int count = 5)
        {
            if (count <= 0) return new List<PopularServerInfo>();
            if (count > 50) count = 50; // !!!

            return servers.Values
                          .OrderByDescending(s => s.Stats.averageMatchesPerDay)
                          .Take(count)
                          .Select(s => new PopularServerInfo
                          {
                              endpoint = s.endpoint,
                              name = s.info.name,
                              averageMatchesPerDay = s.Stats.averageMatchesPerDay
                          })
                          .ToList();
        }
    }
}
