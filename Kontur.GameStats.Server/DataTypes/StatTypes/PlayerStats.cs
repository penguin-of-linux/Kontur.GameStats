using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

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

        private readonly Dictionary<string, int> _serversVisits = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _gameModesPlays = new Dictionary<string, int>();
        private readonly Dictionary<int, int> _matchesPerDay = new Dictionary<int, int>();

        private double _summScoreboardPercent;
        private int _kills;
        private int _deaths;

        public PlayerStats(string name)// : this()
        {
            Name = name;
        }

        public void Update(MatchInfo match, string server, string time)
        {
            totalMatchesPlayed++;

            if (Name == match.Players.First()) totalMatchesWon++;

            ConsiderVisitedServer(server);

            ConsiderGameMode(match.gameMode);

            _summScoreboardPercent += GetScoreboardPercent(match);
            averageScoreboardPercent = _summScoreboardPercent / totalMatchesPlayed;

            ConsiderTime(time);

            averageMatchesPerDay = (double) totalMatchesPlayed / _matchesPerDay.Keys.Count;

            lastMatchPlayed = time;

            ComputeKillsToDeathsRatio(match);
        }

        private void ConsiderVisitedServer(string server)
        {
            if (!_serversVisits.ContainsKey(server))
            {
                _serversVisits[server] = 0;
                uniqueServers++;
            }

            _serversVisits[server]++;

            if (favoriteServer != null)
            {
                if (_serversVisits[favoriteServer] < _serversVisits[server])
                    favoriteServer = server;
            }
            else favoriteServer = server;
        }

        private void ConsiderGameMode(string gameMode)
        {
            if (!_gameModesPlays.ContainsKey(gameMode))
                _gameModesPlays[gameMode] = 0;
            _gameModesPlays[gameMode]++;

            if (favoriteGameMode != null)
            {
                if (_gameModesPlays[favoriteGameMode] < _gameModesPlays[gameMode])
                    favoriteGameMode = gameMode;
            }
            else favoriteGameMode = gameMode;
        }

        private void ConsiderTime(string time)
        {
            var day = DateTime.Parse(time).Day;

            if (!_matchesPerDay.ContainsKey(day))
                _matchesPerDay[day] = 0;

            var matchesCount = ++_matchesPerDay[day];

            if (maximumMatchesPerDay < matchesCount)
                maximumMatchesPerDay = matchesCount;
        }

        private void ComputeKillsToDeathsRatio(MatchInfo match)
        {
            var name = Name;
            try
            {
                var scoreBoardUnit = match.scoreBoard.Single(sbu => sbu.name.ToLower() == name);
                _kills = scoreBoardUnit.kills;
                _deaths = scoreBoardUnit.deaths;
            }
            catch (InvalidOperationException)
            {
                throw new PlayerNotFoundException($"Player '{name}' not found");
            }

            if (_deaths == 0) killToDeathRatio = int.MaxValue;
            else killToDeathRatio = (double) _kills / _deaths;
        }

        private double GetScoreboardPercent(MatchInfo match)
        {
            var players = match.scoreBoard.Count;
            for (int i = 0; i < players; i++)
                if (match.scoreBoard[i].name.ToLower() == Name)
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