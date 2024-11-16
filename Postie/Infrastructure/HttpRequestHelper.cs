namespace Postie.Infrastructure
{
    public sealed class HttpRequestHelper
    {
        private static readonly Lazy<HttpRequestHelper> _instance = new Lazy<HttpRequestHelper>(() => new HttpRequestHelper());

        private HttpRequestHelper() 
        {
            DefaultHttpClient = new HttpClient();
            DefaultHttpClient.Timeout = TimeSpan.FromSeconds(5);
        }

        public HttpClient DefaultHttpClient;

        public static HttpRequestHelper Instance => _instance.Value;
    }
}
