using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Kontur.GameStats.Server
{
    [DataContract]
    public class ServerStats
    {
        [DataMember] public int totalMatchesPlayed;
        [DataMember] public int maximumPopulation;
        [DataMember] public int maximumMatchesPerDay;
        [DataMember] public double averageMatchesPerDay;
        [DataMember] public double averagePopulation;
        [DataMember] public List<string> top5GameModes = new List<string>();
        [DataMember] public List<string> top5Maps = new List<string>();

        private readonly Dictionary<int, int> _matchesPerDay;        // Maximum 14 days
        private int _totalPlayerCount;
        private readonly Dictionary<string, int> _gameModesFrequency;
        private readonly Dictionary<string, int> _mapsFrequency;


        public ServerStats()
        {
            _matchesPerDay = new Dictionary<int, int>();
            _gameModesFrequency = new Dictionary<string, int>();
            _mapsFrequency = new Dictionary<string, int>();
        }

        public void Update(string time, MatchInfo match)
        {
            totalMatchesPlayed++;

            var day = DateTime.Parse(time).Day;

            if (!_matchesPerDay.ContainsKey(day)) _matchesPerDay[day] = 0;
            if (!_gameModesFrequency.ContainsKey(match.gameMode)) _gameModesFrequency[match.gameMode] = 0;
            if (!_mapsFrequency.ContainsKey(match.map)) _mapsFrequency[match.map] = 0;

            _gameModesFrequency[match.gameMode]++;
            _mapsFrequency[match.map]++;
            var matchPerDay = ++_matchesPerDay[day];

            if (maximumMatchesPerDay < matchPerDay)
                maximumMatchesPerDay = matchPerDay;

            var playerCount = match.PlayersCount;
            _totalPlayerCount += playerCount;

            if (maximumPopulation < playerCount)
                maximumPopulation = playerCount;

            averageMatchesPerDay = (double)totalMatchesPlayed / _matchesPerDay.Keys.Count;
            averagePopulation = (double)_totalPlayerCount / totalMatchesPlayed;
            top5GameModes = TopSelector(_gameModesFrequency, 5);
            top5Maps = TopSelector(_mapsFrequency, 5);
        }

        private List<string> TopSelector(Dictionary<string, int> dict, int count)
        {
            if (dict == null) return new List<string>();
            return dict.OrderByDescending(kvp => kvp.Value)
                       .Select(kvp => kvp.Key)
                       .Take(count)
                       .ToList();
        }

        public override int GetHashCode()
        {
            return totalMatchesPlayed.GetHashCode()
                 + maximumMatchesPerDay.GetHashCode()
                 + maximumPopulation.GetHashCode()
                 + averageMatchesPerDay.GetHashCode()
                 + averagePopulation.GetHashCode()
                 + top5GameModes.Sum(gm => gm.GetHashCode())
                 + top5Maps.Sum(m => m.GetHashCode());
        }

        public override bool Equals(object other)
        {
            if (!(other is ServerStats)) return false;
            return this.GetHashCode() == ((ServerStats) other).GetHashCode();
        }
    }
}