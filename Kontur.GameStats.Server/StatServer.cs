using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kontur.GameStats.Server
{
    public class StatServer : IDisposable
    {
        public StatServer()
        {
            listener = new HttpListener();
            controller = new DataBaseController();
        }
        
        public void Start(string prefix)
        {
            lock (listener)
            {
                if (!isRunning)
                {
                    listener.Prefixes.Clear();
                    listener.Prefixes.Add(prefix);
                    listener.Start();

                    listenerThread = new Thread(Listen)
                    {
                        IsBackground = true,
                        Priority = ThreadPriority.Highest
                    };
                    listenerThread.Start();
                    
                    isRunning = true;
                }
            }
        }

        public void Stop()
        {
            lock (listener)
            {
                if (!isRunning)
                    return;

                listener.Stop();

                listenerThread.Abort();
                listenerThread.Join();
                
                isRunning = false;
            }
        }

        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;

            Stop();

            listener.Close();
        }
        
        private void Listen()
        {
            while (true)
            {
                try
                {
                    if (listener.IsListening)
                    {
                        var context = listener.GetContext();
                        Task.Run(() => HandleContextAsync(context));
                    }
                    else Thread.Sleep(0);
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (Exception error)
                {
                    // TODO: log errors
                }
            }
        }

        private async Task HandleContextAsync(HttpListenerContext listenerContext)
        {
            // TODO: implement request handling

            var text = new StreamReader(listenerContext.Request.InputStream).ReadToEnd();
            Console.WriteLine(text);
            //var headers = listenerContext.Request.Headers;

            /*listenerContext.Response.StatusCode = (int)HttpStatusCode.OK;
            using (var writer = new StreamWriter(listenerContext.Response.OutputStream))
            {
                var req = listenerContext.Request;
                //foreach(var header in headers)
                writer.WriteLine($"{req.RawUrl} {req.HttpMethod} {text}");
            }
            return;*/
            Console.WriteLine("Обработка");
            var request = listenerContext.Request;

            var method = DataBaseController.GetMethod(request.HttpMethod);
            var commandParameter = request.RawUrl;

            var stream = new MemoryStream(Encoding.Unicode.GetBytes(text));

            try
            {
                Console.WriteLine("Перед контроллером");
                var result = controller.HandleRequest(method, commandParameter, stream);
                Console.WriteLine("После контроллера");

                var response = listenerContext.Response;
                //response.OutputStream.Write(data, 0, data.Length);
                Console.WriteLine("Запись");
                using (var writer = new StreamWriter(listenerContext.Response.OutputStream))
                {
                    using (var reader = new StreamReader(result.Item2))
                    {
                        result.Item2.Position = 0;
                        var s = reader.ReadToEnd();
                        //var s = Encoding.Unicode.GetString(result.Item2.);
                        writer.WriteLine(s + " END " + result.Item2.Length);
                        Console.WriteLine($"!!!!!!!!!{s}!!!!!!");
                    }

                }
                Console.WriteLine("Конец записи");
            }
            catch (Exception e)
            {
                Console.WriteLine("Catched");
                using (var writer = new StreamWriter(listenerContext.Response.OutputStream))
                {
                    writer.WriteLine("ERROR " + e.ToString());
                }
                return;
            }

        }

        private readonly HttpListener listener;

        private Thread listenerThread;
        private bool disposed;
        private volatile bool isRunning;
        private DataBaseController controller;
    }
}