using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;


namespace Kontur.GameStats.Server
{
    public class DataBase
    {
        private Dictionary<string, Server> servers;
        private Dictionary<string, PlayerStats> playersStats;
        private SortedDictionary<DateTime, RecentMatchInfo> matches;

        public DataBase()
        {
            servers = new Dictionary<string, Server>();
            playersStats = new Dictionary<string, PlayerStats>();
            matches = new SortedDictionary<DateTime, RecentMatchInfo>(new DescendingComparer<DateTime>());
        }

        public void AddServer(Server server)
        {
            servers[server.Endpoint] = server;
        }

        public void AddServer(string endpoint, string name, List<string> gameModes)
        {
            AddServer(new Server(endpoint, name, gameModes));
        }

        public List<Server> GetAllServers()
        {
            return new List<Server>(servers.Values);
        }

        public List<MatchInfo> GetAllMatches()
        {
            return new List<MatchInfo>(matches.Values.Select(x => x.Results));
        }

        public Server GetServer(string endpoint)
        {
            return servers[endpoint];
        }

        public void PutMatch(string endpoint, string time, MatchInfo match)
        {
            var dateTime = DateTime.Parse(time);
            matches[dateTime] = new RecentMatchInfo { Server = endpoint, Timestamp = time, Results = match };  // !!!!!!!!!!!!!!!!!!!!
            servers[endpoint].Matches[dateTime] = match;
            GetServerStats(endpoint).Update(dateTime, match);

            foreach (var player in match.Players)
            {
                if (!playersStats.ContainsKey(player))
                    playersStats[player] = new PlayerStats(player);

                playersStats[player].Update(match, endpoint, time);
            }
        }

        public MatchInfo GetMatch(string endpoint, string time)
        {
            return GetMatch(endpoint, DateTime.Parse(time));
        }

        public MatchInfo GetMatch(string endpoint, DateTime time)
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

            return matches.Take(count).Select(kvp => kvp.Value).ToList();
        }

        public List<BestPlayerInfo> GetBestPlayers(int count = 5)
        {
            if (count <= 0) return new List<BestPlayerInfo>();
            if (count > 50) count = 50; // !!!

            return playersStats.Values
                               .OrderByDescending(ps => ps.KillToDeathRatio)
                               .Take(count)
                               .Select(ps => new BestPlayerInfo { Name = ps.Name, KillToDeathRatio = ps.KillToDeathRatio })
                               .ToList();
        }
    }
}
