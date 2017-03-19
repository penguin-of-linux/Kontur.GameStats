using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

namespace Kontur.GameStats.Server
{
    [DataContract]
    public struct BestPlayerInfo
    {
        [DataMember] public string name;
        [DataMember] public double killToDeathRatio;
    }
}
