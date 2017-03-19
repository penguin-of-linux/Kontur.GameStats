using System;
using System.IO;
using System.Net;
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
            /*Console.WriteLine(text);
            //var headers = listenerContext.Request.Headers;

            listenerContext.Response.StatusCode = (int)HttpStatusCode.OK;
            using (var writer = new StreamWriter(listenerContext.Response.OutputStream))
            {
                var req = listenerContext.Request;
                //foreach(var header in headers)
                writer.WriteLine("hello");
            }
            return;*/

            Console.WriteLine("Обработка");
            var request = listenerContext.Request;

            var method = DataBaseController.GetMethod(request.HttpMethod);
            var commandParameter = request.RawUrl;

            var stream = new MemoryStream(Encoding.Unicode.GetBytes(text));

            Tuple<HttpStatusCode, Stream> result;

            try
            {
                result = controller.HandleRequest(method, commandParameter, stream);
            }
            catch (Exception e)
            {
                Console.WriteLine("Captured exception " + e);
                result = controller.HandleException(e);
            }
            Console.WriteLine("After");
            using (var writer = new StreamWriter(listenerContext.Response.OutputStream))
            {
                using (var reader = new StreamReader(result.Item2))
                {
                    listenerContext.Response.StatusCode = (int) result.Item1;
                    result.Item2.Position = 0;
                    var answer = reader.ReadToEnd();
                    writer.WriteLine("Answer: " + answer);
                }
            }

        }

        private readonly HttpListener listener;

        private Thread listenerThread;
        private bool disposed;
        private volatile bool isRunning;
        private DataBaseController controller;
    }
}