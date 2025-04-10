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
            return queryFilters
                .GetType()
                .GetProperties()
                .Select(
                    propertyValue =>
                    {
                        var builder = new StringBuilder();
                        var value = propertyValue.GetValue(queryFilters);

                        if (value is PageFilterRequest pageFilter)
                        {
                            builder.Append($"&PageFilter.PageSize={pageFilter.PageSize}");
                        }

                        if (value is IEnumerable<Guid> enumerableGuid)
                        {
                            builder.AppendJoin("&", enumerableGuid.Select(item => $"{propertyValue.Name}=" + item.ToString()));
                        }

                        if (value is IEnumerable<string> enumerableString)
                        {
                            builder.AppendJoin("&", enumerableString.Select(item => $"{propertyValue.Name}=" + item));
                        }

                        if (value is string basicString)
                        {
                            builder.AppendJoin("&", $"{propertyValue.Name}=" + basicString);
                        }

                        if (value is DateTime dateTime && dateTime != DateTime.MinValue)
                        {
                            builder
                            .Append(propertyValue.Name)
                            .Append('=')
                            .Append(dateTime.Year)
                            .Append('-')
                            .Append(dateTime.Month)
                            .Append('-')
                            .Append(dateTime.Day);
                        }

                        return builder.ToString();
                    });
        }
    }
}
