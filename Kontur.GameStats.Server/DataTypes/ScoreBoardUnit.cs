using System.Runtime.Serialization;

namespace Kontur.GameStats.Server
{
    [DataContract]
    public struct ScoreBoardUnit
    {
        [DataMember] public readonly string name;
        [DataMember] public readonly int frags;
        [DataMember] public readonly int kills;
        [DataMember] public readonly int deaths;

        public ScoreBoardUnit(string name, int frags, int kills, int deaths)
        {
            this.name = name;
            this.frags = frags;
            this.kills = kills;
            this.deaths = deaths;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ScoreBoardUnit)) return false;
            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return name.GetHashCode() +
                   frags +
                   kills +
                   deaths;
        }
    }
}