namespace HttpDispatchProxyExtension
{
    public class BaseHttpMethod : Attribute
    {
        public BaseHttpMethod(string path)
        {
            Path = path;
        }
        public string Path { get; }
        public HttpMethod Method { get; set; }
    }
    public class POST : BaseHttpMethod
    {
        public POST(string path = "") : base(path)
        {
            Method = HttpMethod.Post;
        }
    }
    public class GET : BaseHttpMethod
    {
        public GET(string path = "") : base(path)
        {
            Method = HttpMethod.Get;
        }
    }
}