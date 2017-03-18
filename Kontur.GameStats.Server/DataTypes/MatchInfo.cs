using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Kontur.GameStats.Server
{
    [DataContract]
    public struct MatchInfo : IDataType
    {
        [DataMember] public readonly string Map;
        [DataMember] public readonly string GameMode;
        [DataMember] public readonly int FragLimit;
        [DataMember] public readonly int TimeLimit;
        [DataMember] public readonly double TimeElapsed;
        [DataMember] public readonly List<ScoreBoardUnit> ScoreBoard;

        public int PlayersCount => ScoreBoard?.Count ?? 0;
        public List<string> Players => ScoreBoard?.Select(sbu => sbu.Name).ToList();

        public MatchInfo(string map, 
                         string gameMode,
                         int fragLimit, 
                         int timeLimit, 
                         double timeElapsed,
                         List<ScoreBoardUnit> scoreBoard)
        {
            Map = map;
            GameMode = gameMode;
            FragLimit = fragLimit;
            TimeLimit = timeLimit;
            TimeElapsed = timeElapsed;
            ScoreBoard = scoreBoard;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MatchInfo)) return false;
            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return (int)(Map.GetHashCode() +
                         GameMode.GetHashCode() +
                         FragLimit +
                         TimeLimit +
                         TimeElapsed +
                         ScoreBoard?.Sum(u => u.GetHashCode()) ?? 0);
        }
    }
}