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
        [DataMember] public int TotalMatchesPlayed;
        [DataMember] public int MaximumPopulation;
        [DataMember] public int MaximumMatchesPerDay;
        [DataMember] public double AverageMatchesPerDay;
        [DataMember] public double AveragePopulation;
        [DataMember] public List<string> Top5GameModes = new List<string>();
        [DataMember] public List<string> Top5Maps = new List<string>();

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

        public void Update(DateTime time, MatchInfo match)
        {
            TotalMatchesPlayed++;

            var day = time.Day;

            if (!matchesPerDay.ContainsKey(day)) matchesPerDay[day] = 0;
            if (!gameModesFrequency.ContainsKey(match.GameMode)) gameModesFrequency[match.GameMode] = 0;
            if (!mapsFrequency.ContainsKey(match.Map)) mapsFrequency[match.Map] = 0;

            gameModesFrequency[match.GameMode]++;
            mapsFrequency[match.Map]++;
            var matchPerDay = ++matchesPerDay[day];

            if (MaximumMatchesPerDay < matchPerDay)
                MaximumMatchesPerDay = matchPerDay;

            var playerCount = match.PlayersCount;
            totalPlayerCount += playerCount;

            if (MaximumPopulation < playerCount)
                MaximumPopulation = playerCount;

            AverageMatchesPerDay = (double)TotalMatchesPlayed / matchesPerDay.Keys.Count;
            AveragePopulation = (double)totalPlayerCount / TotalMatchesPlayed;
            Top5GameModes = TopSelector(gameModesFrequency, 5);
            Top5Maps = TopSelector(mapsFrequency, 5);
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
            return TotalMatchesPlayed.GetHashCode()
                 + MaximumMatchesPerDay.GetHashCode()
                 + MaximumPopulation.GetHashCode()
                 + AverageMatchesPerDay.GetHashCode()
                 + AveragePopulation.GetHashCode()
                 + Top5GameModes.Sum(gm => gm.GetHashCode())
                 + Top5Maps.Sum(m => m.GetHashCode());
        }

        public override bool Equals(object other)
        {
            if (!(other is ServerStats)) return false;
            return this.GetHashCode() == ((ServerStats) other).GetHashCode();
        }
    }
}