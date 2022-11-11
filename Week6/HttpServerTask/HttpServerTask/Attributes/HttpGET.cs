namespace HttpServerTask.Attributes;

internal class HttpGET : HttpRequest
{
    public HttpGET(string methodUri = "") : base(methodUri)
    {
    }
}