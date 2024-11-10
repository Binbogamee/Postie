using System.Net.Http;
using System.Net.Http.Headers;

namespace Postie.Configurations
{
    public sealed class RequestClient
    {
        // Ленивое создание экземпляра через Lazy<T>
        private static readonly Lazy<RequestClient> _instance = new Lazy<RequestClient>(() => new RequestClient());

        // Закрытый конструктор, чтобы нельзя было создать экземпляр снаружи
        private RequestClient() 
        {
            DefaultHttpClient = new HttpClient();
            DefaultHttpClient.Timeout = TimeSpan.FromSeconds(5);
            //DefaultHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public HttpClient DefaultHttpClient;

        // Публичное свойство для доступа к экземпляру
        public static RequestClient Instance => _instance.Value;
    }
}
