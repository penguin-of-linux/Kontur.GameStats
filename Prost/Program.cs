using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using Kontur.GameStats.Server;

namespace Prost
{

    public class Program
    {
        public static void Main(string[] args)
        {
            //var address = args[0]; //"http://localhost:8080/servers/1-8080/info";
            var data = File.ReadAllText("cmd.txt");
            var uri = File.ReadAllText("uri.txt");

            HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(uri);
            request2.Method = "PUT";

            using (var writer = new StreamWriter(request2.GetRequestStream()))
            {
                writer.Write(data.ToString());
            }

            WebResponse response2 = request2.GetResponse();
            using (var stream = new StreamReader(response2.GetResponseStream()))
            {
                var r = $"Input:\"{stream.ReadToEnd()}\"";
                Console.WriteLine(r);
            }
            response2.Close();
        }
    }
}
