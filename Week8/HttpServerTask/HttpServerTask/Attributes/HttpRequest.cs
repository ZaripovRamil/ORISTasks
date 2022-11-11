namespace HttpServerTask.Attributes
{
    internal abstract class HttpRequest : Attribute
    {
        public string MethodUri;
        protected HttpRequest(string methodUri)
        {
            MethodUri = methodUri;
        }
    }
}