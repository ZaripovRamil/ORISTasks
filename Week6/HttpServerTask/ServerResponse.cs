using System.Text;

namespace HttpServerTask;

public class ServerResponse
{
    public readonly byte[] Buffer;
    public readonly string ContentType;

    public ServerResponse(string path, string rawUrl)
    {
        if (!Directory.Exists(path))
        {
            (Buffer, ContentType) = (Encoding.UTF8.GetBytes($"Directory {path} does not exist"), "text/plain");
            return;
        }

        var buffer = GetFile(path + rawUrl.Replace("%20", " "));
        var contentType = GetContentType(rawUrl);
        if (buffer.Length == 0)
        {
            contentType = "text/plain";
            buffer = Encoding.UTF8.GetBytes($"File {path} not found");
        }

        Buffer = buffer;
        ContentType = contentType;
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