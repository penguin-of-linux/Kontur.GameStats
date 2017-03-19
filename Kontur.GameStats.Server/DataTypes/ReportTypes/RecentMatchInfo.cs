using System.Runtime.Serialization;

namespace Kontur.GameStats.Server
{
    [DataContract]
    public struct RecentMatchInfo
    {
        [DataMember] public string server;
        [DataMember] public string timestamp;
        [DataMember] public MatchInfo results;
    }
}
