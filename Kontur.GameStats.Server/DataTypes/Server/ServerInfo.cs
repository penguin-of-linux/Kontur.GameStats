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
        [DataMember] public readonly string Name;
        [DataMember] public readonly List<string> GameModes;

        public ServerInfo(string name, List<string> gameModes)
        {
            Name = name;
            GameModes = gameModes;
        }

        public ServerInfo(string name, params string[] gameModes)
        {
            this = new ServerInfo(name, new List<string>(gameModes));
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() + (GameModes?.Sum(m => m.GetHashCode()) ?? 0);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ServerInfo)) return false;

            return GetHashCode() == obj.GetHashCode();
        }
    }
}
