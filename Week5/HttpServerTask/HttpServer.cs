using System.Net;

namespace HttpServerTask;

public class HttpServer
{
    static void Main(string[] args)
    {
        var listener = StartServerListener();
        HandlingServerListenerRequests(listener);
    }

    private static void HandlingServerListenerRequests(HttpListener listener)
    {
        try
        {
            while (true)
            {
                Console.WriteLine("Ожидание подключений...");
                var context = listener.GetContext();
                HandleRequest(context);
            }
        }
        catch(Exception e)
        {
            Console.WriteLine($"An exception has been thrown. {e.Message}. Restart server(y/n)?");
            var answer = Console.ReadLine();
            if (answer == "y") RestartServerListener(listener);
            else StopServerListener(listener);
        }
    }

    private static void HandleRequest(HttpListenerContext context)
    {
        var request = context.Request;
        if (request.Url.LocalPath == "/google")
        {
            var response = context.Response;
            var responseStr = File.ReadAllText("google\\google.html");
            var buffer = System.Text.Encoding.UTF8.GetBytes(responseStr);
            response.ContentLength64 = buffer.Length;
            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }
    }

    private static void RestartServerListener(HttpListener listener)
    {
        listener.Stop();
        listener = StartServerListener();
        HandlingServerListenerRequests(listener);
    }


    private static void StopServerListener(HttpListener listener)
    {
        listener.Close();
    }

    private static HttpListener StartServerListener()
    {
        var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8888/");
        listener.Start();
        return listener;
    }
}