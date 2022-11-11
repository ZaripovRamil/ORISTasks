namespace HttpServerTask;

public static class Program
{
    private static ServerStatus _status;

    public static void Main()
    {
        var server = new HttpServer();
        var start = new Task(() => server.Start());
        start.Start();
        _status = ServerStatus.Active;
        while (_status == ServerStatus.Active)
            HandleConsoleCommand(Console.ReadLine() ?? string.Empty, server);
        Console.WriteLine("Program finished");
    }

    private static void HandleConsoleCommand(string command, HttpServer server)
    {
        if(ConsoleCommands.ContainsKey(command))
            ConsoleCommands[command].Invoke(server);
        else ConsoleCommands["Incorrect"].Invoke(server);
    }

    private static readonly Dictionary<string, Action<HttpServer>> ConsoleCommands = new()
    {
        {
            "Status", server => Console.WriteLine(server.Status)
        },
        {
            "Start", server => server.Start()
        },
        {
            "Stop", server => server.Stop()
        },
        {
            "Restart", server =>
            {
                server.Stop();
                server.Start();
            }
        },
        {
            "Exit", server =>
            {
                server.Stop();
                _status = ServerStatus.Dead;
            }
        },
        {
            "Incorrect", _ =>
            {
                Console.WriteLine("incorrect command");
            }
        }
    };
}