namespace Qualia.Hermes
{
    public class HttpResponse : HttpMessageWrapper<HttpResponseMessage, HttpResponse>
    { 
        public async Task<T?> ReceiveJson<T>()
        {
            string? body = await ReceiveString();
            return System.Text.Json.JsonSerializer.Deserialize<T>(body ?? string.Empty);
        }
        public async Task<string?> ReceiveString()
        {
            Message.EnsureSuccessStatusCode();
            string body = await Message.Content.ReadAsStringAsync();
            return body;
        }
    }

    /// <summary>
    /// Extensions to expose HttpResponse through its Task wrapper
    /// </summary>
    public static partial class Extentions
    {
        public async static Task<T?> ReceiveJson<T>(this Task<HttpResponse> response)
        {
            await response;
            return await response.Result.ReceiveJson<T>();
        }
        public async static Task<string?> ReceiveString(this Task<HttpResponse> response)
        {
            await response;
            return await response.Result.ReceiveString();
        }
    }
}
