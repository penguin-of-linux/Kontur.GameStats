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
        [DataMember] public int TotalMatchesPlayed;
        [DataMember] public int TotalMatchesWon;
        [DataMember] public string FavoriteServer;
        [DataMember] public int UniqueServers;
        [DataMember] public string FavoriteGameMode;
        [DataMember] public double AverageScoreboardPercent;
        [DataMember] public int MaximumMatchesPerDay;
        [DataMember] public double AverageMatchesPerDay;
        [DataMember] public string LastMatchPlayed;
        [DataMember] public double KillToDeathRatio;

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
            TotalMatchesPlayed++;

            if (Name == match.ScoreBoard.First().Name) TotalMatchesWon++;

            if (!serversVisits.ContainsKey(server))
            {
                serversVisits[server] = 0;
                UniqueServers++;
            }

            serversVisits[server]++;

            if (FavoriteServer != null)
            {
                if (serversVisits[FavoriteServer] < serversVisits[server])
                    FavoriteServer = server;
            }
            else FavoriteServer = server;

            if (!gameModesPlays.ContainsKey(match.GameMode))
                gameModesPlays[match.GameMode] = 0;
            gameModesPlays[match.GameMode]++;

            if (FavoriteGameMode != null)
            {
                if (gameModesPlays[FavoriteGameMode] < gameModesPlays[match.GameMode])
                    FavoriteGameMode = match.GameMode;
            }
            else FavoriteGameMode = match.GameMode;

            summScoreboardPercent += GetScoreboardPercent(match);
            AverageScoreboardPercent = summScoreboardPercent / TotalMatchesPlayed;

            var day = DateTime.Parse(time).Day;
            if (!matchesPerDay.ContainsKey(day))
                matchesPerDay[day] = 0;
            var matchesCount = ++matchesPerDay[day];
            if (MaximumMatchesPerDay < matchesCount)
                MaximumMatchesPerDay = matchesCount;

            AverageMatchesPerDay = (double) TotalMatchesPlayed / matchesPerDay.Keys.Count;

            LastMatchPlayed = time;

            var name = Name;
            kills = match.ScoreBoard.Single(sbu => sbu.Name == name).Kills;
            deaths = match.ScoreBoard.Single(sbu => sbu.Name == name).Deaths;
            KillToDeathRatio = (double) kills / deaths;
        }

        private double GetScoreboardPercent(MatchInfo match)
        {
            var players = match.ScoreBoard.Count;
            for (int i = 0; i < players; i++)
                if (match.ScoreBoard[i].Name == Name)
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
            return TotalMatchesPlayed.GetHashCode() + 
                   TotalMatchesWon.GetHashCode() + 
                   FavoriteServer.GetHashCode() + 
                   UniqueServers.GetHashCode() + 
                   FavoriteGameMode.GetHashCode() + 
                   AverageScoreboardPercent.GetHashCode() + 
                   MaximumMatchesPerDay.GetHashCode() + 
                   AverageMatchesPerDay.GetHashCode() + 
                   LastMatchPlayed.GetHashCode() + 
                   KillToDeathRatio.GetHashCode();
        }
    }
}