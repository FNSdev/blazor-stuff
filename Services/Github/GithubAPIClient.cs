using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Text.Json;
using System.IO;
using hephaestus.Models;
using hephaestus.Services.RequestContents;


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

        private int perPageLimit = 5;

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

        private void AddHeaders(HttpRequestMessage request)
        {
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", $"token {token}");
            request.Headers.Add("User-Agent", "Hephaestus App");
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
            AddHeaders(request);

            var response = await makeRequest(request);
            if(response.ErrorMessage != null)
            {
                throw new GithubAPIClientException(response.ErrorMessage);
            }

            var getUserInfoResponse = await JsonSerializer.DeserializeAsync<GetUserInfoResponse>(response.Response);
            return getUserInfoResponse;
        }

        public async Task<GetUserRepositoriesResponse[]> GetUserRepositories(int page = 1)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"{baseAPIUrl}/user/repos?page={page}&per_page={perPageLimit}&type=owner");
            AddHeaders(request);

            var response = await makeRequest(request);
            if (response.ErrorMessage != null)
            {
                throw new GithubAPIClientException(response.ErrorMessage);
            }

            var getUserRepositoriesResponse =
                await JsonSerializer.DeserializeAsync<GetUserRepositoriesResponse[]>(response.Response);
            return getUserRepositoriesResponse;
        }

        public async Task<CreateWebhookResponse> CreateWebhook(string owner, string repoName, string action)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, 
                $"{baseAPIUrl}/repos/{owner}/{repoName}/hooks");
            var content = new CreateWebhookRequestContent
            {
                config = new CreateWebhookRequestContent.Config
                {
                    url=$"{_config["Common:BaseUrl"]}/Webhook/{action}"
                },
                events = new [] {action.ToLower()},
            };
            var rawContent = new StringContent(JsonSerializer.Serialize(content, null));
            request.Content = rawContent;
            AddHeaders(request);

            var response = await makeRequest(request);
            if(response.ErrorMessage != null)
            {
                throw new GithubAPIClientException(response.ErrorMessage);
            }
            var createWebhookResponse = await JsonSerializer.DeserializeAsync<CreateWebhookResponse>(response.Response);       
            return createWebhookResponse;      
        }
    }
}
