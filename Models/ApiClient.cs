using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
//Alex Marcouiller

namespace Final.Models
{
    public class ApiClient : IDisposable
    {
        private string _urlBaseApi;
        private HttpClient _httpClient;

        public ApiClient(string urlBaseApi)
        {
            if (urlBaseApi.EndsWith('/'))
                urlBaseApi = urlBaseApi.Substring(0, urlBaseApi.Length - 1);
            _urlBaseApi = urlBaseApi;

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void SetHttpRequestHeader(string header, string val)
        {
            _httpClient.DefaultRequestHeaders.Remove(header);
            _httpClient.DefaultRequestHeaders.Add(header, val);
        }

        public async Task<string> RequeteGetAsync(string endpoint)
        {

            HttpResponseMessage hrm = await _httpClient.GetAsync(_urlBaseApi + endpoint);
            return await hrm.Content.ReadAsStringAsync();
        }

        public async Task<string> RequetePostAsync(string endpoint, string json)
        {
            var donnees = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage hrm = await _httpClient.PostAsync(_urlBaseApi + endpoint, donnees);
            return await hrm.Content.ReadAsStringAsync();
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
