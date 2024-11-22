using AW.Infrastructure.Interfaces.Services;
using AW.Infrastructure.Utils;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AW.Infrastructure.Services
{
    public class ClientCredentialService : IClientCredentialService
    {
        private const int MaxRetries = 3;
        private readonly IPrincipal _user;
        private readonly HttpClient _httpClient;

        public ClientCredentialService(IPrincipal user, HttpClient httpClient)
        {
            _user = user;
            _httpClient = httpClient;
        }

        public async Task<TResponse?> GetAsync<TResponse>(string uri, Dictionary<string, string> headers)
        {
            return await CallAPI<TResponse>(Method.Get, uri, headers);
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string uri, Dictionary<string, string> headers, TRequest data)
        {
            return await CallAPI<TResponse>(Method.Post, uri, headers, data);
        }

        public async Task<TResponse?> PutAsync<TRequest, TResponse>(string uri, Dictionary<string, string> headers, TRequest data)
        {
            return await CallAPI<TResponse>(Method.Put, uri, headers, data);
        }

        public async Task<TResponse?> DeleteAsync<TResponse>(string uri, Dictionary<string, string> headers)
        {
            return await CallAPI<TResponse>(Method.Delete, uri, headers);
        }

        private async Task<dynamic?> CallAPI<TResponse>(Method method, string uri, Dictionary<string, string> headers, object? data = null, bool needAuthentication = false)
        {
            int tries = 0;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                //string userId = Helper.getValueFromClaims(_user, "nameidentifier", _user.Identity?.Name ?? "");

                using (var request = new HttpRequestMessage())
                {
                    switch (method)
                    {
                        case Method.Get: request.Method = HttpMethod.Get; break;
                        case Method.Post: request.Method = HttpMethod.Post; break;
                        case Method.Put: request.Method = HttpMethod.Put; break;
                        case Method.Delete: request.Method = HttpMethod.Delete; break;
                    }
                    request.RequestUri = new Uri(uri);

                    //if (needAuthentication) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "xxxxxxxxxxxxxxxxxxxx");

                    foreach (var json in headers.ToList())
                    {
                        request.Headers.Add(json.Key, json.Value);
                    }

                    if (data != null) request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

                    response = await _httpClient.SendAsync(request);
                }

                //response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(content);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to call {method} API using Client Credential. | Retried for {(tries - 1)} times | URL: {uri} | Error Content: {ex.Message}");
            }
        }
    }
}
