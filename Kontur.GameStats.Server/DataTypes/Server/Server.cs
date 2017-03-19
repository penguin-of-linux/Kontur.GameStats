using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Kontur.GameStats.Server
{
    [DataContract]
    public struct Server : IDataType
    {
        [DataMember] public readonly string endpoint;
        [DataMember] public readonly ServerInfo info;

        public readonly Dictionary<string, MatchInfo> Matches;

        internal ServerStats Stats;// => GetServerStats();

        public Server(string endpoint, string name, params string[] gameModes) 
            : this(endpoint, new ServerInfo(name, new List<string>(gameModes)))
        {
        }

        public Server(string endpoint, ServerInfo serverInfo)
        {
            this.endpoint = endpoint;
            info = serverInfo;
            Matches = new Dictionary<string, MatchInfo>();
            Stats = new ServerStats();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ServerInfo)) return false;
            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return endpoint.GetHashCode() + info.GetHashCode();
        }
    }
}