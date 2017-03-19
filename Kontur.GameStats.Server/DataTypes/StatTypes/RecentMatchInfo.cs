using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
