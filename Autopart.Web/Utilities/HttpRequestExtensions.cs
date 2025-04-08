using System.Text;

namespace ZikApp.API.Utilities
{
    public static class HttpRequestExtensions
    {
        public static async Task<string> GetRawBodyStringAsync(this HttpRequest request, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            request.Body.Position = 0L;
            using StreamReader reader = new(request.Body, encoding);
            return await reader.ReadToEndAsync();
        }
    }
}
