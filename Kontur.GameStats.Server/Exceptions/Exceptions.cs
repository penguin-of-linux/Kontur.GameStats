using System;

namespace Kontur.GameStats.Server
{
    public class ServerNotFoundException : Exception
    {
        public ServerNotFoundException(string message) : base(message) { }
    }
    public class PlayerNotFoundException : Exception
    {
        public PlayerNotFoundException(string message) : base(message) { }
    }
}