namespace Postie.Configurations
{
    public sealed class RequestClient
    {
        private static readonly Lazy<RequestClient> _instance = new Lazy<RequestClient>(() => new RequestClient());

        private RequestClient() 
        {
            DefaultHttpClient = new HttpClient();
            DefaultHttpClient.Timeout = TimeSpan.FromSeconds(5);
        }

        public HttpClient DefaultHttpClient;

        public static RequestClient Instance => _instance.Value;
    }
}
