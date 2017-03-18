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
            var rnd = new Random();
            var l = new List<int>();
            Console.WriteLine(DateTime.Now);
            for(int i=0 ; i < 1000*1000*1; i++)
                l.Add(rnd.Next(0, 100));

            var start = DateTime.Now;
            Console.WriteLine(start);
            var a = l.OrderByDescending(i => i).ToList();
            Console.WriteLine(DateTime.Now - start);
            return;
            var address = "http://localhost:8080/servers/myserver-8080/info";
            var str = "{\"name\": \"] My P3rfect Server [\",\"gameModes\": [\"DM\", \"TDM\"]}";
            //var data = Encoding.UTF8.GetBytes(str);
            //var server = new ServerInfo("My SERVAK", "DM", "TDM");
            //var dataStream = new MemoryStream();

            //Console.WriteLine(HTTP_PUT(address, str).Results);

            /*using (var client = new WebClient())
            {
                var b = new WebClient().UploadData(address, "PUT",
                    data);
                Console.WriteLine(b.Length);
            }*/


            HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(address);
            request2.Method = "PUT";
            //request2.ContentType = "application/json";
            //request2.ContentLength = data.Length;

            using (var writer = new StreamWriter(request2.GetRequestStream()))
            {
                writer.Write(str);
            }

            //Serializer.SerializeObject(server, request2.GetRequestStream());
            //using (var sendStream = request2.GetRequestStream())
            //{
            //    sendStream.Write(data, 0, data.Length);
            //    sendStream.Close();
            //}

            WebResponse response2 = request2.GetResponse();
            using (var stream = new StreamReader(response2.GetResponseStream()))
            {
                var r = $"Input:\"{stream.ReadToEnd()}\"";
                Console.WriteLine(r);
            }
            response2.Close();
            ////////////////////////
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:8080/servers/info");
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Method = "GET";

            WebResponse response = request.GetResponse();
            using (var stream = new StreamReader(response.GetResponseStream()))
            {
                var r = $"Input:\"{stream.ReadToEnd()}\"";
                Console.WriteLine(r);
            }
            response.Close();
        }

        //JsonResultModel class
        public class JsonResultModel
        {
            public string ErrorMessage { get; set; }
            public bool IsSuccess { get; set; }
            public string Results { get; set; }
        }
        // HTTP_PUT Function
        public static JsonResultModel HTTP_PUT(string Url, string Data)
        {
            JsonResultModel model = new JsonResultModel();
            string Out = String.Empty;
            string Error = String.Empty;
            System.Net.WebRequest req = System.Net.WebRequest.Create(Url);

            try
            {
                req.Method = "PUT";
                req.Timeout = 100000;
                req.ContentType = "application/json";
                byte[] sentData = Encoding.UTF8.GetBytes(Data);
                req.ContentLength = sentData.Length;

                using (System.IO.Stream sendStream = req.GetRequestStream())
                {
                    sendStream.Write(sentData, 0, sentData.Length);
                    sendStream.Close();

                }

                System.Net.WebResponse res = req.GetResponse();
                System.IO.Stream ReceiveStream = res.GetResponseStream();
                using (System.IO.StreamReader sr = new
                System.IO.StreamReader(ReceiveStream, Encoding.UTF8))
                {

                    Char[] read = new Char[256];
                    int count = sr.Read(read, 0, 256);

                    while (count > 0)
                    {
                        String str = new String(read, 0, count);
                        Out += str;
                        count = sr.Read(read, 0, 256);
                    }
                }
            }
            catch (ArgumentException ex)
            {
                Error = string.Format("HTTP_ERROR :: The second HttpWebRequest object has raised an Argument Exception as 'Connection' Property is set to 'Close' :: {0}", ex.Message);
            }
            catch (WebException ex)
            {
                Error = string.Format("HTTP_ERROR :: WebException raised! :: {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Error = string.Format("HTTP_ERROR :: Exception raised! :: {0}", ex.Message);
            }

            model.Results = Out;
            model.ErrorMessage = Error;
            if (!string.IsNullOrWhiteSpace(Out))
            {
                model.IsSuccess = true;
            }
            return model;
        }
    }

    [DataContract]
    class Person
    {
        [DataMember]
        private int age;

        [DataMember]
        private string name;

        public Person(string name = "Vasya", int age = 20)
        {
            this.age = age;
            this.name = name;
        }
    }
}
