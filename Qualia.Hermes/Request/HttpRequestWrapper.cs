using System.Text;

namespace Qualia.Hermes
{
    public class HttpRequestWrapper
    {
        #region [PROPS]
        public HttpRequestMessage HttpRequestMessage { get; private set; }
        private HttpClient _httpClient { get; }
        #endregion

        #region [CTOR - FACTORY]
        private HttpRequestWrapper(HttpRequestMessage msg, HttpClient httpClient)
        {
            HttpRequestMessage = msg;
            _httpClient = httpClient;
        }

        public static HttpRequestWrapper From(HttpRequestMessage msg, HttpClient httpClient)
        {
            var instance = new HttpRequestWrapper(msg, httpClient);
            return instance;
        }
        #endregion

        #region [METHODS]
        public HttpRequestWrapper Uri(Uri uri)
        {
            HttpRequestMessage.RequestUri = uri;
            return this;
        }
        public HttpRequestWrapper Uri(string uri)
        {
            HttpRequestMessage.RequestUri = new Uri(uri);
            return this;
        }
        public HttpRequestWrapper WithQuery(System.Collections.Specialized.NameValueCollection nvc)
        {
            var pairs = nvc.AllKeys.SelectMany(
                k => nvc.GetValues(k) ?? new string[] { }, 
                (key, value) => string.Format(
                    "{0}={1}",
                    System.Web.HttpUtility.UrlEncode(key),
                    System.Web.HttpUtility.UrlEncode(value)
                    )
                ).ToArray();
            return WithQuery("?" + string.Join("&", pairs));
        }
        public HttpRequestWrapper WithQuery(string query)
        {
            var old_uri = HttpRequestMessage.RequestUri ?? throw new ArgumentNullException(nameof(HttpRequestMessage.RequestUri));
            var ub = new UriBuilder(
                old_uri.Scheme,
                old_uri.Host,
                old_uri.Port,
                old_uri.AbsolutePath, 
                query
                );
            HttpRequestMessage.RequestUri = ub.Uri;
            return this;
        }
        public HttpRequestWrapper WithHeaders(System.Collections.Specialized.NameValueCollection? headers)
        {
            headers?
                .AllKeys?
                .Where(x => x != null)
                .ToList()
                .ForEach(
                    k => HttpRequestMessage.Headers.Add(k, headers[k])
                    );
            return this;
        }
        public async Task<HttpResponseWrapper> PostAsJsonAsync<TRequest>(TRequest? data = default(TRequest))
        {
            var content = 
                new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(data), 
                    Encoding.UTF8, 
                    System.Net.Mime.MediaTypeNames.Application.Json
                    );
            return await PostAsync(content);
        }
        public async Task<HttpResponseWrapper> PostAsync(HttpContent? httpContent = null)
        {
            HttpRequestMessage.Method = HttpMethod.Post;
            HttpRequestMessage.Content = httpContent;
            return await SendAsync();
        }
        public async Task<HttpResponseWrapper> GetAsync()
        {
            HttpRequestMessage.Method = HttpMethod.Get;
            return await SendAsync();
        }
        private async Task<HttpResponseWrapper> SendAsync()
        {
            HttpResponseMessage? response = await _httpClient.SendAsync(HttpRequestMessage);
            return HttpResponseWrapper.From(response);
        }
        #endregion

    }
}
