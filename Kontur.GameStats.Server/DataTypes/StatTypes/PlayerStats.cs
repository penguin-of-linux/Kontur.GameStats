using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Policy;

namespace Kontur.GameStats.Server
{
    [DataContract]
    public class PlayerStats
    {
        [DataMember] public int totalMatchesPlayed;
        [DataMember] public int totalMatchesWon;
        [DataMember] public string favoriteServer;
        [DataMember] public int uniqueServers;
        [DataMember] public string favoriteGameMode;
        [DataMember] public double averageScoreboardPercent;
        [DataMember] public int maximumMatchesPerDay;
        [DataMember] public double averageMatchesPerDay;
        [DataMember] public string lastMatchPlayed;
        [DataMember] public double killToDeathRatio;

        public readonly string Name;

        private Dictionary<string, int> serversVisits;
        private Dictionary<string, int> gameModesPlays;
        private Dictionary<int, int> matchesPerDay;

        private double summScoreboardPercent;
        private int kills;
        private int deaths;

        private void Init()
        {
            serversVisits = new Dictionary<string, int>();
            gameModesPlays = new Dictionary<string, int>();
            matchesPerDay = new Dictionary<int, int>();
        }

        public PlayerStats(string name)// : this()
        {
            Init();
            Name = name;
        }

        public void Update(MatchInfo match, string server, string time)
        {
            totalMatchesPlayed++;

            if (Name == match.scoreBoard.First().name) totalMatchesWon++;

            if (!serversVisits.ContainsKey(server))
            {
                serversVisits[server] = 0;
                uniqueServers++;
            }

            serversVisits[server]++;

            if (favoriteServer != null)
            {
                if (serversVisits[favoriteServer] < serversVisits[server])
                    favoriteServer = server;
            }
            else favoriteServer = server;

            if (!gameModesPlays.ContainsKey(match.gameMode))
                gameModesPlays[match.gameMode] = 0;
            gameModesPlays[match.gameMode]++;

            if (favoriteGameMode != null)
            {
                if (gameModesPlays[favoriteGameMode] < gameModesPlays[match.gameMode])
                    favoriteGameMode = match.gameMode;
            }
            else favoriteGameMode = match.gameMode;

            summScoreboardPercent += GetScoreboardPercent(match);
            averageScoreboardPercent = summScoreboardPercent / totalMatchesPlayed;

            var day = DateTime.Parse(time).Day;
            if (!matchesPerDay.ContainsKey(day))
                matchesPerDay[day] = 0;
            var matchesCount = ++matchesPerDay[day];
            if (maximumMatchesPerDay < matchesCount)
                maximumMatchesPerDay = matchesCount;

            averageMatchesPerDay = (double) totalMatchesPlayed / matchesPerDay.Keys.Count;

            lastMatchPlayed = time;

            var name = Name;
            kills = match.scoreBoard.Single(sbu => sbu.name == name).kills;
            deaths = match.scoreBoard.Single(sbu => sbu.name == name).deaths;
            killToDeathRatio = (double) kills / deaths;
        }

        private double GetScoreboardPercent(MatchInfo match)
        {
            var players = match.scoreBoard.Count;
            for (int i = 0; i < players; i++)
                if (match.scoreBoard[i].name == Name)
                    return (double)(players - i - 1) / (players - 1) * 100;
            throw new ArgumentException("MatchInfo hasn't player");
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PlayerStats)) return false;

            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return totalMatchesPlayed.GetHashCode() + 
                   totalMatchesWon.GetHashCode() + 
                   favoriteServer.GetHashCode() + 
                   uniqueServers.GetHashCode() + 
                   favoriteGameMode.GetHashCode() + 
                   averageScoreboardPercent.GetHashCode() + 
                   maximumMatchesPerDay.GetHashCode() + 
                   averageMatchesPerDay.GetHashCode() + 
                   lastMatchPlayed.GetHashCode() + 
                   killToDeathRatio.GetHashCode();
        }
    }
}