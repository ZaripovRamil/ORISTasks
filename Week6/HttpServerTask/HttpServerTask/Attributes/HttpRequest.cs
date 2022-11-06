namespace HttpServerTask.Attributes
{
    abstract class HttpRequest : Attribute
    {
        public string MethodURI;
        protected HttpRequest(string methodURI)
        {
            MethodURI = methodURI;
        }
        protected HttpRequest()
        {
            MethodURI = "";
        }
    }
}