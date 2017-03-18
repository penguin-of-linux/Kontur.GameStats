﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kontur.GameStats.Server.Commands
{
    public class RecentMatchesCommand : ICommand, IGet
    {
        private readonly DataBase dataBase;

        public RecentMatchesCommand(DataBase dataBase)
        {
            this.dataBase = dataBase;
        }

        public bool IsParametersDefineCommand(string commandParameters)
        {
            var parameters = commandParameters.Split('/');
            return parameters[1] == "reports" && parameters[2] == "recent-matches";
        }

        public Stream Get(string commandParameter)
        {
            var dataStream = new MemoryStream();
            var parameters = commandParameter.Split('/');
            var count = parameters.Length == 3 ? 5 : int.Parse(parameters[3]);
            //var count = int.Parse(commandParameter.Split('/')[3] ?? "5");

            Serializer.SerializeObject(dataBase.GetRecentMatches(count), dataStream);

            return dataStream;
        }
    }
}
