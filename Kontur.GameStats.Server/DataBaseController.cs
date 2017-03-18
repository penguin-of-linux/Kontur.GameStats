using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Kontur.GameStats.Server.Commands;

namespace Kontur.GameStats.Server
{
    public class DataBaseController
    {
        private List<ICommand> commands;

        private DataBase dataBase;

        public DataBaseController(DataBase dataBase = null)
        {
            this.dataBase = dataBase ?? new DataBase();
            commands = new List<ICommand>()
            {
                new EndpointInfoCommand(this.dataBase),
                new MatchInfoCommand(this.dataBase),
                new ServersInfoCommand(this.dataBase),
                new ServerStatsCommand(this.dataBase),
                new PlayerStatsCommand(this.dataBase),
                new RecentMatchesCommand(this.dataBase),
                new BestPlayersCommand(this.dataBase)
            };
        }
        
        public static MethodType GetMethod(string method)
        {
            switch (method)
            {
                case "GET": return MethodType.Get;
                case "PUT": return MethodType.Put;
                default: throw new ArgumentException("Unknown method");
            }
        }

        public Tuple<StatusCode, Stream> HandleRequest(MethodType method, string commandParameter, Stream dataStream = null)
        {
            Stream resultStream = new MemoryStream();
            foreach (var command in commands)
            {
                if (!command.IsParametersDefineCommand(commandParameter)) continue;

                switch (method)
                {
                    case MethodType.Get:
                        resultStream = ((IGet)command).Get(commandParameter);
                        return new Tuple<StatusCode, Stream>(StatusCode.OK, resultStream);

                    case MethodType.Put:
                        if (dataStream == null) ; // Обработать! (возможно, перегрузкой и проверкой на null)
                        /*var success = */((IPut)command).Put(commandParameter, dataStream);
                        return new Tuple<StatusCode, Stream>(StatusCode.OK, resultStream);

                    default:
                        throw new Exception("Unknown method type");
                }
            }

            throw new Exception("Unknown command type");
        }
    }
}
