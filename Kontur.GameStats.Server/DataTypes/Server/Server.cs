using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Fclp.Internals.Extensions;

namespace Kontur.GameStats.Server
{
    [DataContract]
    public struct Server : IDataType
    {
        [DataMember] public readonly string Endpoint;
        [DataMember] public readonly ServerInfo Info;

        public readonly Dictionary<DateTime, MatchInfo> Matches;

        internal ServerStats Stats;// => GetServerStats();

        public Server(string endpoint, string name, List<string> gameModes) 
            : this(endpoint, new ServerInfo(name, gameModes))
        {
        }

        public Server(string endpoint, string name, params string[] gameModes) 
            : this(endpoint, new ServerInfo(name, new List<string>(gameModes)))
        {
        }

        public Server(string endpoint, ServerInfo serverInfo)
        {
            Endpoint = endpoint;
            Info = serverInfo;
            Matches = new Dictionary<DateTime, MatchInfo>();
            Stats = new ServerStats();
        }

        /*public ServerStats GetServerStats()
        {
            lock (this)
            {
                var tm = matches.Values.Count;
                var populationPerMatch = matches.Values.Select(m => m.PlayersCount).ToList();
                var mp = populationPerMatch.Max();
                var ap = (double)populationPerMatch.Sum() / populationPerMatch.Count();
                var tempMatches = matches;
                var matchesPerDay = matches.Keys
                                           .Select(dt => tempMatches
                                               .Count(kvp => kvp.Key.Day == dt.Day))
                                           .ToList();

                var mmpd = matchesPerDay.Max();
                var ampd = (double)matchesPerDay.Sum() / matchesPerDay.Count();
                var t5gm = matches.Values
                                  .Select(m => m.GameMode)
                                  .Select(gm => new Tuple<string, int>(gm,
                                                tempMatches.Values.Count(m => m.GameMode == gm)))
                                  .OrderByDescending(t => t.Item2)
                                  .Take(5)
                                  .Select(t => t.Item1)
                                  .ToList();

                var t5m = matches.Values
                                  .Select(m => m.Map)
                                  .Select(map => new Tuple<string, int>(map,
                                                tempMatches.Values.Count(m => m.Map == map)))
                                  .OrderByDescending(t => t.Item2)
                                  .Take(5)
                                  .Select(t => t.Item1)
                                  .ToList();
                return new ServerStats(tm, mp, mmpd, ampd, ap, t5gm, t5m);
            }

        }*/

        public override bool Equals(object obj)
        {
            if (!(obj is ServerInfo)) return false;
            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return Endpoint.GetHashCode() +
                   Info.GetHashCode(); //+
            //matches?.Keys.Sum(k => k.GetHashCode()) ?? 0 +
            //matches?.Values.Sum(v => v.GetHashCode()) ?? 0;
        }
    }
}