namespace HttpServerTask.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class ApiController : Attribute
    {
        public string ModelUri;

        public ApiController(string modelUri)
        {
            ModelUri = modelUri;
        }
    }
}