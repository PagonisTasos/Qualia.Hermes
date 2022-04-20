namespace Qualia.Hermes
{
    public static partial class Extentions
    {
        public static HttpRequestWrapper RequestBuilder(this HttpClient httpClient)
        {
            HttpRequestWrapper request = HttpRequestWrapper.From(new HttpRequestMessage(), httpClient);
            return request;
        }
    }
}
