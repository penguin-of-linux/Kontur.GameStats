using System;
using System.Collections.Generic;
using System.Linq;

namespace Kontur.GameStats.Server
{
    public class SimpleDataBase
    {
        private readonly Dictionary<string, Server> _servers;
        private readonly Dictionary<string, PlayerStats> _playersStats;

        public SimpleDataBase()
        {
            _servers = new Dictionary<string, Server>();
            _playersStats = new Dictionary<string, PlayerStats>();
        }

        public void AddServer(Server server)
        {
            _servers[server.endpoint] = server;
        }

        public List<Server> GetAllServers()
        {
            return new List<Server>(_servers.Values);
        }

        public List<MatchInfo> GetAllMatches()
        {
            return new List<MatchInfo>(_servers.Values.SelectMany(s => s.Matches.Values));
        }

        public Server GetServer(string endpoint)
        {
            if(!_servers.ContainsKey(endpoint))
                throw new ServerNotFoundException($"Server '{endpoint}' not found");
            return _servers[endpoint];
        }

        public void PutMatch(string endpoint, string time, MatchInfo match)
        {
            GetServer(endpoint).Matches[time] = match;
            GetServerStats(endpoint).Update(time, match);

            foreach (var player in match.Players)
            {
                if (!_playersStats.ContainsKey(player))
                    _playersStats[player] = new PlayerStats(player);

                _playersStats[player].Update(match, endpoint, time);
            }
        }

        public MatchInfo GetMatch(string endpoint, string time)
        {
            return GetServer(endpoint).Matches[time];
        }

        public ServerStats GetServerStats(string endpoint)
        {
            return GetServer(endpoint).Stats;
        }

        public PlayerStats GetPlayerStats(string name)
        {
            return _playersStats[name.ToLower()];
        }

        public List<RecentMatchInfo> GetRecentMatches(int count = 5)
        {
            if (count <= 0) return new List<RecentMatchInfo>();
            if (count > 50) count = 50; // !!!

            return _servers.Values
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

            return _playersStats.Values
                                .Where(pl => pl.totalMatchesPlayed >= ServerOptions.MatchCountNeededToBeBestPlayer || 
                                             (int) pl.killToDeathRatio != int.MaxValue)
                                .OrderByDescending(ps => ps.killToDeathRatio)
                                .Take(count)
                                .Select(ps => new BestPlayerInfo { name = ps.Name, killToDeathRatio = ps.killToDeathRatio })
                                .ToList();
        }

        public List<PopularServerInfo> GetPopularServers(int count = 5)
        {
            if (count <= 0) return new List<PopularServerInfo>();
            if (count > 50) count = 50; // !!!

            return _servers.Values
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
