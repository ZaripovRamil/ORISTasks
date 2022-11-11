using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using HttpServerTask.Attributes;

namespace HttpServerTask;

public class ServerResponse
{
    public readonly byte[] Buffer;
    public readonly string ContentType;
    public HttpStatusCode Status;

    public ServerResponse(string path, HttpListenerRequest request)
    {
        if (!Directory.Exists(path))
        {
            (Status, Buffer, ContentType) = (HttpStatusCode.NotFound,
                Encoding.UTF8.GetBytes($"Directory {path} does not exist"), "text/plain");
            return;
        }

        var buffer = GetFile(path + request.RawUrl?.Replace("%20", " "));
        var contentType = GetContentType(request.RawUrl);
        if (buffer.Length != 0)
        {
            Buffer = buffer;
            ContentType = contentType;
            Status = HttpStatusCode.OK;
            return;
        }

        if (TryHandleController(request, out Buffer))
        {
            Status = Buffer.Length > 0 ? HttpStatusCode.OK : HttpStatusCode.Redirect;
            ContentType = "application/json";
            return;
        }

        Status = HttpStatusCode.NotFound;
        ContentType = "text/plain";
        Buffer = Encoding.UTF8.GetBytes($"File {path} not found");
    }

    private static byte[] GetFile(string filePath)
    {
        if (Directory.Exists(filePath))
        {
            filePath += "/index.html";
            if (File.Exists(filePath))
                return File.ReadAllBytes(filePath);
        }

        if (File.Exists(filePath))
            return File.ReadAllBytes(filePath);
        return Array.Empty<byte>();
    }

    private static bool TryHandleController(HttpListenerRequest request, out byte[] buffer)
    {
        buffer = Array.Empty<byte>();
        if (request.Url!.Segments.Length < 2) return false;

        using var sr = new StreamReader(request.InputStream, request.ContentEncoding);
        var controllerName = request.Url.Segments[1].Replace("/", "");
        var strParams = request.Url.Segments
            .Skip(2)
            .Select(s => s.Replace("/", ""))
            .Concat(sr.ReadToEnd().Split('&').Select(p => p.Split('=').LastOrDefault()))
            .ToArray();

        var assembly = Assembly.GetExecutingAssembly();
        var controller = assembly.GetTypes()
            .Where(t => Attribute.IsDefined(t, typeof(ApiController)))
            .FirstOrDefault(t => string.Equals(
                (t.GetCustomAttribute(typeof(ApiController)) as ApiController)?.ModelUri,
                controllerName,
                StringComparison.CurrentCultureIgnoreCase));

        var method = controller?.GetMethods()
            .FirstOrDefault(t => t.GetCustomAttributes(true)
                .Any(attr => attr.GetType().Name == $"Http{request.HttpMethod}"
                             && Regex.IsMatch(request.RawUrl ?? "",
                                 attr.GetType()
                                     .GetField("MethodURI")?
                                     .GetValue(attr)?.ToString() ?? "")));
        if (method is null) return false;

        var queryParams = method.GetParameters()
            .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
            .ToArray();
        var ctor = controller.GetConstructors()[0];
        var ret = method.Invoke(ctor.Invoke(new []{"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=steamDB;Integrated Security=True;"}), queryParams);
        buffer = request.HttpMethod == "POST" ? buffer : Encoding.ASCII.GetBytes(JsonSerializer.Serialize(ret));

        return true;
    }

    private static string GetContentType(string path)
    {
        var ext = path.Contains('.') ? path.Split('.')[^1] : "html";
        return ContentTypes.ContainsKey(ext) ? ContentTypes[ext] : "text/plain";
    }

    private static readonly Dictionary<string, string> ContentTypes = new()
    {
        {"txt", "text/plain"},
        {"jpg", "image/jpeg"},
        {"png", "image/png"},
        {"gif", "image/gif"},
        {"svg", "image/svg+xml"},
        {"css", "text/css"},
        {"html", "text/html"}
    };
}