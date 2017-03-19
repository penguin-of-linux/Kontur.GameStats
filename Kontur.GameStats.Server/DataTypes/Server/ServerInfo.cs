using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kontur.GameStats.Server
{
    [DataContract]
    public struct ServerInfo
    {
        [DataMember] public readonly string name;
        [DataMember] public readonly List<string> gameModes;

        public ServerInfo(string name, List<string> gameModes)
        {
            this.name = name;
            this.gameModes = gameModes;
        }

        public ServerInfo(string name, params string[] gameModes)
        {
            this = new ServerInfo(name, new List<string>(gameModes));
        }

        public override int GetHashCode()
        {
            return name.GetHashCode() + (gameModes?.Sum(m => m.GetHashCode()) ?? 0);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ServerInfo)) return false;

            return GetHashCode() == obj.GetHashCode();
        }
    }
}
