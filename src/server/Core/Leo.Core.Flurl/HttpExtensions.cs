using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Flurl.Http
{
    public static class HttpExtensions
    {
        public static Task<T> GetJsonAsync<T>(this Url url, string route, object request, CancellationToken cancellationToken = default(CancellationToken), HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            return url.WithParams(route, request).GetJsonAsync<T>(cancellationToken, completionOption);
        }

        public static Task<T> GetJsonAsync<T>(this string url, string route, object request, CancellationToken cancellationToken = default(CancellationToken), HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            return url.WithParams(route, request).GetJsonAsync<T>(cancellationToken, completionOption);
        }

        public static Task<T> GetJsonAsync<T>(this IFlurlClient client, string route, object request, CancellationToken cancellationToken = default(CancellationToken), HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            var routeUrl = route.WithParams(request);
            var clientRequest = client.Request(routeUrl.Path);
            foreach (var queryParam in routeUrl.QueryParams)
            {
                clientRequest.SetQueryParam(queryParam.Name, queryParam.Value);
            }
            
            return clientRequest.GetJsonAsync<T>(cancellationToken, completionOption: completionOption);
        }

        public static async Task<T> PostJsonAsync<T>(this Url url, string route, object request, CancellationToken cancellationToken = default(CancellationToken), HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            var response = await url.WithParams(route, request, true).PostJsonAsync(request, cancellationToken, completionOption: completionOption);
            return await response.Content.ReadAs<T>();
        }

        public static async Task<T> PostJsonAsync<T>(this string url, string route, object request, CancellationToken cancellationToken = default(CancellationToken), HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            var response = await url.WithParams(route, request, true).PostJsonAsync(request, cancellationToken, completionOption: completionOption);
            return await response.Content.ReadAs<T>();
        }

        public static async Task<T> PostJsonAsync<T>(this IFlurlClient client, string route, object request, CancellationToken cancellationToken = default(CancellationToken), HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            var routeUrl = route.WithParams(request, true);
            var response = await client.Request(routeUrl.Path).PostJsonAsync(request, cancellationToken, completionOption: completionOption);
            return await response.Content.ReadAs<T>();
        }

        public static async Task<T> ReadAs<T>(this HttpContent content)
        {
            var json = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static Task<T> RequestJsonAsync<T>(this FlurlClient client, object request, CancellationToken cancellationToken = default(CancellationToken), HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            var attributes = request.GetType().GetCustomAttributes(typeof(RequestRouteAttribute), false);
            if (attributes != null && attributes.Any())
            {
                var requestRoute = attributes.First() as RequestRouteAttribute;
                switch (requestRoute.Method.ToLower())
                {
                    case "get":
                        return client.GetJsonAsync<T>(requestRoute.RouteTemplate, request, cancellationToken, completionOption);

                    default:
                        return client.PostJsonAsync<T>(requestRoute.RouteTemplate, request, cancellationToken, completionOption);
                }
            }
            else
                throw new ArgumentException("Request dto must have RequestRoute attribute to use with SendAsync.");
        }

        public static Task<T> RequestJsonAsync<T>(this Url url, object request, CancellationToken cancellationToken = default(CancellationToken), HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            return new FlurlClient(url).RequestJsonAsync<T>(request, cancellationToken, completionOption);
        }

        public static Task<T> RequestJsonAsync<T>(this string url, object request, CancellationToken cancellationToken = default(CancellationToken), HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            return new FlurlClient(url).RequestJsonAsync<T>(request, cancellationToken, completionOption);
        }
    }
}