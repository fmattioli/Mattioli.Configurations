using Flurl;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Coderaw.Settings.Http
{
    public class BaseHttpClient(HttpClient httpClient)
    {
        private readonly HttpClient _httpClient = httpClient;

        protected async Task<TResponse> GetAsync<TRequest, TResponse>(
            string path,
            TRequest request,
            CancellationToken cancellationToken)
            where TResponse : class
            where TRequest : class
        {
            var queryParams = BuildFilters(request);

            var url = _httpClient.BaseAddress
                .AppendPathSegment(path)
                .AppendQueryParam(queryParams);

            var response = await _httpClient.GetAsync(url, cancellationToken);
            var result = JsonConvert.DeserializeObject<TResponse>(await response.Content.ReadAsStringAsync(cancellationToken));

            return result!;
        }

        protected async Task<TResponse> GetAsync<TResponse>(
            string path,
            CancellationToken cancellationToken)
            where TResponse : class
        {

            var url = _httpClient.BaseAddress
                .AppendPathSegment(path);

            var response = await _httpClient.GetAsync(url, cancellationToken);
            var result = JsonConvert.DeserializeObject<TResponse>(await response.Content.ReadAsStringAsync(cancellationToken));

            return result!;
        }

        protected async Task<TResponse> GetAsync<TResponse>(
            string queryParams,
            string jwtToken,
            CancellationToken cancellationToken)
            where TResponse : class
        {

            var url = _httpClient!.BaseAddress!.ToString() + queryParams;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.GetAsync(url, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonConvert.DeserializeObject<TResponse>(content);

            return result!;
        }

        protected async Task<TResponse> PostAsync<TRequest, TResponse>(
            string path,
            TRequest request,
            CancellationToken cancellationToken)
            where TResponse : class
            where TRequest : class
        {
            var url = _httpClient!.BaseAddress!.ToString().AppendPathSegment(path);

            string json = JsonConvert.SerializeObject(request);

            StringContent bodyContent = new(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, bodyContent, cancellationToken);

            var result = JsonConvert.DeserializeObject<TResponse>(await response.Content.ReadAsStringAsync(cancellationToken));
            return result!;
        }

        private static IEnumerable<string> BuildFilters<T>(T queryFilters) where T : class
        {
            if (queryFilters == null)
                yield break;

            foreach (var property in queryFilters.GetType().GetProperties())
            {
                var value = property.GetValue(queryFilters);
                if (value == null)
                    continue;

                var name = property.Name;

                switch (value)
                {
                    case PageFilterRequest pageFilter:
                        yield return $"PageFilter.PageSize={pageFilter.PageSize}";
                        break;

                    case IEnumerable<Guid> guids:
                        foreach (var guid in guids)
                            yield return $"{name}={guid}";
                        break;

                    case IEnumerable<string> strings:
                        foreach (var str in strings)
                            yield return $"{name}={str}";
                        break;

                    case string str:
                        yield return $"{name}={str}";
                        break;

                    case int _:
                    case short _:
                    case decimal _:
                    case float _:
                        yield return $"{name}={value}";
                        break;

                    case DateTime dt when dt != DateTime.MinValue:
                        yield return $"{name}={dt:yyyy-MM-dd}";
                        break;
                }
            }
        }
    }
}
