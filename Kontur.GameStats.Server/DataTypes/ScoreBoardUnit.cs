using System.Runtime.Serialization;

namespace Kontur.GameStats.Server
{
    [DataContract]
    public struct ScoreBoardUnit
    {
        [DataMember] public readonly string Name;
        [DataMember] public readonly int Frags;
        [DataMember] public readonly int Kills;
        [DataMember] public readonly int Deaths;

        public ScoreBoardUnit(string name, int frags, int kills, int deaths)
        {
            Name = name;
            Frags = frags;
            Kills = kills;
            Deaths = deaths;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ScoreBoardUnit)) return false;
            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() +
                   Frags +
                   Kills +
                   Deaths;
        }
    }
}