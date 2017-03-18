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
        [DataMember] public string Server;
        [DataMember] public string Timestamp;
        [DataMember] public MatchInfo Results;
    }
}
