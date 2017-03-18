using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kontur.GameStats.Server.DataTypes.ReportTypes
{
    [DataContract]
    public struct PopularServerInfo
    {
        [DataMember] public string Endpoint;
        [DataMember] public string Name;
        [DataMember] public double AverageMatchesPerDay;
    }
}
