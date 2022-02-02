using System.Text;

namespace Qualia.Hermes
{
    public class HttpRequest : HttpMessageWrapper<HttpRequestMessage, HttpRequest>
    {
        public HttpRequest WithQuery(System.Collections.Specialized.NameValueCollection nvc)
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
        public HttpRequest WithQuery(string query)
        {
            var old_uri = Message.RequestUri ?? throw new ArgumentNullException(nameof(Message.RequestUri));
            var ub = new UriBuilder(
                old_uri.Scheme,
                old_uri.Host,
                old_uri.Port,
                old_uri.AbsolutePath, 
                query
                );
            Message.RequestUri = ub.Uri;
            return this;
        }
        public HttpRequest WithHeaders(System.Collections.Specialized.NameValueCollection? headers)
        {
            headers?
                .AllKeys?
                .Where(x => x != null)
                .ToList()
                .ForEach(
                    k => Message.Headers.Add(k, headers[k])
                    );
            return this;
        }
        public async Task<HttpResponse> PostAsJsonAsync<TRequest>(TRequest? data = default(TRequest))
        {
            var content = 
                new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(data), 
                    Encoding.UTF8, 
                    System.Net.Mime.MediaTypeNames.Application.Json
                    );
            return await PostAsync(content);
        }
        public async Task<HttpResponse> PostAsync(HttpContent? httpContent = null)
        {
            Message.Method = HttpMethod.Post;
            Message.Content = httpContent;
            return await SendAsync();
        }
        public async Task<HttpResponse> GetAsync()
        {
            Message.Method = HttpMethod.Get;
            return await SendAsync();
        }
        private async Task<HttpResponse> SendAsync()
        {
            HttpClient httpClient = new();
            HttpResponseMessage? response = await httpClient.SendAsync(Message);
            return HttpResponse.From(response);
        }
    }

    public static partial class Extentions
    {
        public static HttpRequest Request(this Uri uri)
        {
            HttpRequest request = HttpRequest.From(new HttpRequestMessage());
            request.Message.RequestUri = uri;
            return request;
        }
    }
}
