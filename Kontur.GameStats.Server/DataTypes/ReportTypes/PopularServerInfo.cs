using System.Runtime.Serialization;

namespace Kontur.GameStats.Server
{
    [DataContract]
    public struct PopularServerInfo
    {
        [DataMember] public string endpoint;
        [DataMember] public string name;
        [DataMember] public double averageMatchesPerDay;
    }
}
