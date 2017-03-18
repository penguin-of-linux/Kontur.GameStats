using System;

namespace Kontur.GameStats.Server
{
    public static class Common  // rename!!!!!!!!!!!!!!!!!
    {
        // TODO : перевести на regexp?
        public static bool IsEndpoint(string endpoint)
        {
            return endpoint.Split('-').Length == 2; //ну такое
        }

        public static bool IsTimestamp(string timestamp)
        {
            return timestamp.Split('-', ':').Length == 5;
        }
    }
}