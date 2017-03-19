using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Kontur.GameStats.Server
{
    [DataContract]
    public struct MatchInfo : IDataType
    {
        [DataMember] public readonly string map;
        [DataMember] public readonly string gameMode;
        [DataMember] public readonly int fragLimit;
        [DataMember] public readonly int timeLimit;
        [DataMember] public readonly double timeElapsed;
        [DataMember] public readonly List<ScoreBoardUnit> scoreBoard;

        public int PlayersCount => scoreBoard?.Count ?? 0;
        public List<string> Players => scoreBoard?.Select(x => x.name.ToLower()).ToList();

        public MatchInfo(string map, string gameMode, int fragLimit, 
                         int timeLimit, double timeElapsed, List<ScoreBoardUnit> scoreBoard)
        {
            this.map = map;
            this.gameMode = gameMode;
            this.fragLimit = fragLimit;
            this.timeLimit = timeLimit;
            this.timeElapsed = timeElapsed;
            this.scoreBoard = scoreBoard;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MatchInfo)) return false;
            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return (int)(map.GetHashCode() +
                         gameMode.GetHashCode() +
                         fragLimit +
                         timeLimit +
                         timeElapsed +
                         scoreBoard?.Sum(u => u.GetHashCode()) ?? 0);
        }
    }
}