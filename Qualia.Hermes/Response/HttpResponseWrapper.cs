namespace Qualia.Hermes
{
    public class HttpResponseWrapper
    {
        #region [PROPS]
        public HttpResponseMessage HttpResponseMessage { get; private set; }
        #endregion

        #region [CTOR - FACTORY]
        private HttpResponseWrapper(HttpResponseMessage msg)
        {
            HttpResponseMessage = msg;
        }

        public static HttpResponseWrapper From(HttpResponseMessage msg)
        {
            var instance = new HttpResponseWrapper(msg);
            return instance;
        }
        #endregion

        #region [METHODS]
        public async Task<T?> ReceiveJson<T>()
        {
            string? body = await ReceiveString();
            return System.Text.Json.JsonSerializer.Deserialize<T>(body ?? string.Empty);
        }
        public async Task<string?> ReceiveString()
        {
            string body = await HttpResponseMessage.Content.ReadAsStringAsync();
            return body;
        }
        public HttpResponseWrapper EnsureSuccessStatusCode()
        {
            HttpResponseMessage.EnsureSuccessStatusCode();
            return this;
        }
        public async Task<HttpResponseWrapper> HandleResponse(Func<HttpResponseMessage, Task> func)
        {
            await func(HttpResponseMessage);
            return this;
        }
        #endregion
    }
}
