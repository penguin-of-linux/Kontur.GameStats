using System.Runtime.Serialization;

namespace Kontur.GameStats.Server
{
    [DataContract]
    public struct BestPlayerInfo
    {
        [DataMember] public string name;
        [DataMember] public double killToDeathRatio;
    }
}
