using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Text.Json;
using System.IO;


namespace hephaestus.Services
{
    public class GithubAPIClientException : Exception
    {
        /* Base class for all GithubAPIClient exceptions */

        public GithubAPIClientException(string message) : base(message) {}
    }

    public class GithubAPIClient
    {
        private IConfiguration _config;
        private HttpClient _httpClient;
        public string token {get; set;}
        private string baseOauthUrl = "https://github.com/login/oauth/access_token";
        private string baseAPIUrl = "https://api.github.com";

        private class HttpResponse
        {
            public Stream Response {get; set;}
            public string ErrorMessage {get; set;}
        }

        public GithubAPIClient(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
        }

        private async Task<HttpResponse> makeRequest(HttpRequestMessage request)
        {
            try 
            {
                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStreamAsync();

                // TODO use logging
                var s = await response.Content.ReadAsStringAsync();
                Console.WriteLine(s);

                if(!response.IsSuccessStatusCode)
                {
                    var failedRequestResponse = await JsonSerializer.DeserializeAsync<FailedRequestResponse>(responseContent);
                    return new HttpResponse() {Response = null, ErrorMessage = failedRequestResponse.message};
                }


                return new HttpResponse() {Response = responseContent, ErrorMessage = null};
            }
            catch(HttpRequestException exception) 
            {
                return new HttpResponse() {Response = null, ErrorMessage = exception.Message};
            }
        }

        public async Task<GetOauthTokenResponse> GetOauthToken(string code)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, baseOauthUrl);
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("client_id", _config["Github:ClientId"]),
                new KeyValuePair<string, string>("client_secret", _config["Github:ClientSecret"]),
            });
            request.Content = content;
            request.Headers.Add("Accept", "application/json");

            var response = await makeRequest(request);
            if(response.ErrorMessage != null)
            {
                throw new GithubAPIClientException(response.ErrorMessage);
            }
            var getOauthTokenResponse = await JsonSerializer.DeserializeAsync<GetOauthTokenResponse>(response.Response);       
            return getOauthTokenResponse;
        }

        public async Task<GetUserInfoResponse> GetUserInfo()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{baseAPIUrl}/user");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", $"token {token}");
            request.Headers.Add("User-Agent", "Hephaestus App");

            var response = await makeRequest(request);
            if(response.ErrorMessage != null)
            {
                throw new GithubAPIClientException(response.ErrorMessage);
            }

            var getUserInfoResponse = await JsonSerializer.DeserializeAsync<GetUserInfoResponse>(response.Response);
            return getUserInfoResponse;
        }
    }
}