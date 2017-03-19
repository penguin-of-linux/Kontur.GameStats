using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Kontur.GameStats.Server;

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

        /*public ServerStats(int totalMatchesPlayed,
                           int maximumPopulation,
                           int maximumMatchesPerDay,
                           double averageMatchesPerDay,
                           double averagePopulation,
                           List<string> top5GameModes,
                           List<string> top5Maps)
        {
            TotalMatchesPlayed = totalMatchesPlayed;
            MaximumPopulation = maximumPopulation;
            MaximumMatchesPerDay = maximumMatchesPerDay;
            AverageMatchesPerDay = averageMatchesPerDay;
            AveragePopulation = averagePopulation;
            Top5GameModes = top5GameModes;
            Top5Maps = top5Maps;

            matchesPerDay = new Dictionary<int, int>();
            gameModesFrequency = new Dictionary<string, int>();
            mapsFrequency = new Dictionary<string, int>();
        }*/

        private Dictionary<int, int> matchesPerDay;        // Maximum 14 days
        private int totalPlayerCount;
        private Dictionary<string, int> gameModesFrequency;
        private Dictionary<string, int> mapsFrequency;


        public ServerStats()
        {
            matchesPerDay = new Dictionary<int, int>();
            gameModesFrequency = new Dictionary<string, int>();
            mapsFrequency = new Dictionary<string, int>();
        }

        public void Update(string time, MatchInfo match)
        {
            totalMatchesPlayed++;

            var day = DateTime.Parse(time).Day;

            if (!matchesPerDay.ContainsKey(day)) matchesPerDay[day] = 0;
            if (!gameModesFrequency.ContainsKey(match.gameMode)) gameModesFrequency[match.gameMode] = 0;
            if (!mapsFrequency.ContainsKey(match.map)) mapsFrequency[match.map] = 0;

            gameModesFrequency[match.gameMode]++;
            mapsFrequency[match.map]++;
            var matchPerDay = ++matchesPerDay[day];

            if (maximumMatchesPerDay < matchPerDay)
                maximumMatchesPerDay = matchPerDay;

            var playerCount = match.PlayersCount;
            totalPlayerCount += playerCount;

            if (maximumPopulation < playerCount)
                maximumPopulation = playerCount;

            averageMatchesPerDay = (double)totalMatchesPlayed / matchesPerDay.Keys.Count;
            averagePopulation = (double)totalPlayerCount / totalMatchesPlayed;
            top5GameModes = TopSelector(gameModesFrequency, 5);
            top5Maps = TopSelector(mapsFrequency, 5);
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