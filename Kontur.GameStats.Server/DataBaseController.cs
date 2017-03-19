using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Kontur.GameStats.Server.Commands;

namespace Kontur.GameStats.Server
{
    public class DataBaseController
    {
        private List<ICommand> commands;

        private SimpleDataBase dataBase;

        public DataBaseController(SimpleDataBase dataBase = null)
        {
            this.dataBase = dataBase ?? new SimpleDataBase();
            commands = new List<ICommand>()
            {
                new EndpointInfoCommand(this.dataBase),
                new MatchInfoCommand(this.dataBase),
                new ServersInfoCommand(this.dataBase),
                new ServerStatsCommand(this.dataBase),
                new PlayerStatsCommand(this.dataBase),
                new RecentMatchesCommand(this.dataBase),
                new BestPlayersCommand(this.dataBase),
                new PopularServersCommand(this.dataBase)
            };
        }
        
        public static MethodType GetMethod(string method)
        {
            switch (method)
            {
                case "GET": return MethodType.GET;
                case "PUT": return MethodType.PUT;
                default: throw new ArgumentException("Unknown method");
            }
        }

        public Tuple<HttpStatusCode, Stream> HandleRequest(MethodType method, string commandParameter, Stream dataStream = null)
        {
            Stream resultStream = new MemoryStream();
            foreach (var command in commands)
            {
                if (!command.IsParametersDefineCommand(commandParameter)) continue;

                switch (method)
                {
                    case MethodType.GET:
                        resultStream = ((IGetCommand)command).Get(commandParameter);
                        return new Tuple<HttpStatusCode, Stream>(HttpStatusCode.OK, resultStream);

                    case MethodType.PUT:
                        if (dataStream == null || dataStream.Length == 0)
                            throw new ArgumentNullException(nameof(dataStream));
                        ((IPutCommand)command).Put(commandParameter, dataStream);
                        return new Tuple<HttpStatusCode, Stream>(HttpStatusCode.OK, resultStream);

                    default:
                        throw new ArgumentException("Unknown method type");
                }
            }

            throw new ArgumentException("Unknown command type");
        }

        public Tuple<HttpStatusCode, Stream> HandleException(Exception e)
        {
            var message = "Unknown error";
            var status = HttpStatusCode.InternalServerError;

            if (e is ArgumentException || e is KeyNotFoundException)
            {
                message = "Bad request";
                status = HttpStatusCode.BadRequest;
            }

            if (e is ServerNotFoundException)
            {
                message = e.Message;
                status = HttpStatusCode.NotFound;
            }

            var dataStream = new MemoryStream(Encoding.UTF8.GetBytes(message));

            return new Tuple<HttpStatusCode, Stream>(status, dataStream);
        }
    }
}
