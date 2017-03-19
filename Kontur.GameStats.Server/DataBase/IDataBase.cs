using System;
using System.Collections.Generic;

namespace Kontur.GameStats.Server
{
    public interface IDataBase
    {
        void AddServer(Server server);

        List<Server> GetAllServers();

        List<MatchInfo> GetAllMatches();

        Server GetServer(string endpoint);

        void PutMatch(string endpoint, string time, MatchInfo match);

        MatchInfo GetMatch(string endpoint, string time);

        ServerStats GetServerStats(string endpoint);

        PlayerStats GetPlayerStats(string name);

        List<RecentMatchInfo> GetRecentMatches(int count = 5);

        List<BestPlayerInfo> GetBestPlayers(int count = 5);

        List<PopularServerInfo> GetPopularServers(int count = 5);
    }
}
