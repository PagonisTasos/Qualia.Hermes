namespace Qualia.Hermes
{
    /// <summary>
    /// Extensions to expose HttpResponse through its Task wrapper
    /// </summary>
    public static partial class Extentions
    {
        public async static Task<T?> ReceiveJson<T>(this Task<HttpResponseWrapper> response)
        {
            await response;
            return await response.Result.ReceiveJson<T>();
        }
        public async static Task<string?> ReceiveString(this Task<HttpResponseWrapper> response)
        {
            await response;
            return await response.Result.ReceiveString();
        }

        public async static Task<HttpResponseWrapper> HandleResponse(this Task<HttpResponseWrapper> response, Func<HttpResponseMessage, Task> func)
        {
            await response;
            return await response.Result.HandleResponse(func);
        }
    }
}
