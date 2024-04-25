//namespace Modules.Catalog.Infrastructure
//{
//    using System.Net.Http;
//    using System.Text.Json;

//    public static class HttpContentExtensions
//    {
//        private static readonly JsonSerializerOptions DefaultOptions = new()
//        {
//            PropertyNameCaseInsensitive = true,
//            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
//        };

//        public static async Task<T> ReadAsAsync<T>(
//            this HttpContent content,
//            CancellationToken cancellationToken = default)
//        {
//            using (var stream = await content.ReadAsStreamAsync(cancellationToken))
//            {
//                return await JsonSerializer.DeserializeAsync<T>(stream, DefaultOptions, cancellationToken);
//            }
//        }

//        public static async Task<T> ReadAsAsync<T>(
//            this HttpContent content,
//            JsonSerializerOptions options,
//            CancellationToken cancellationToken = default)
//        {
//            using (var stream = await content.ReadAsStreamAsync(cancellationToken))
//            {
//                return await JsonSerializer.DeserializeAsync<T>(stream, options ?? DefaultOptions, cancellationToken);
//            }
//        }
//    }
//}