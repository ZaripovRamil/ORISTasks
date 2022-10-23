using System.Net;
using System.Text.Json;
using static System.GC;

namespace HttpServerTask;

public class HttpServer : IDisposable
{
    public ListenerStatus Status { get; private set; } = ListenerStatus.Dead;
    private ServerSettings _serverSettings;
    private const int MaxListeners = 10;
    private int _activeListeners;

    private readonly HttpListener _httpListener;

    public HttpServer()
    {
        _serverSettings = new ServerSettings();
        _httpListener = new HttpListener();
    }

    public void Start()
    {
        if (Status == ListenerStatus.Active)
        {
            Console.WriteLine("Server is already active.");
            return;
        }
        _serverSettings = JsonSerializer.Deserialize<ServerSettings>(File.OpenRead("./settings.json"))
                          ?? new ServerSettings();
        _httpListener.Prefixes.Clear();
        _httpListener.Prefixes.Add($"http://localhost:{_serverSettings.Port}/");
        _httpListener.Start();
        Console.WriteLine("Server launched");
        Status = ListenerStatus.Active;
        HandleRequests();
    }

    private void HandleRequests()
    {
        try
        {
            while (true)
            {
                if (_activeListeners >= MaxListeners)
                {
                    Thread.Sleep(100);
                    continue;
                }
                _httpListener.BeginGetContext(ListenerCallback, _httpListener);
                _activeListeners += 1;
            }
               
        }
        catch (Exception e)
        {
            Console.WriteLine($"An exception has been thrown. {e.Message}. Restart server(y/n)?");
            var answer = Console.ReadLine();
            if (answer == "y") HandleRequests();
            else Stop();
        }
    }

    public void Stop()
    {
        if (Status == ListenerStatus.Dead)
        {
            Console.WriteLine("Server is already dead");
            return;
        }

        _httpListener.Stop();
        Status = ListenerStatus.Dead;
        Console.WriteLine("Server stopped");
    }

    private void ListenerCallback(IAsyncResult result)
    {
        if (!_httpListener.IsListening) return;
        var httpContext = _httpListener.EndGetContext(result);
        var request = httpContext.Request;
        var response = httpContext.Response;
        var serverResponse = new ServerResponse(_serverSettings.Path, request.RawUrl ?? "/");
        var buffer = serverResponse.Buffer;
        response.Headers.Set("Content-Type", serverResponse.ContentType);
        var output = response.OutputStream;
        var task = output.WriteAsync(buffer, 0, buffer.Length);
        task.Wait();
        _activeListeners -= 1;
        output.Close();
        response.Close();
    }

    public void Dispose()
    {
        Stop();
        SuppressFinalize(this);
    }
}