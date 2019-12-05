using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

using hephaestus.Models;

namespace hephaestus.Services
{
    public class GithubService
    {
        private IConfiguration _config;
        private GithubAPIClient _apiClient;
        private DatabaseContext _databaseContext;
        public string token {get; set;}

        public GithubService(IConfiguration config, GithubAPIClient apiClient, DatabaseContext databaseContext)
        {
            _config = config;
            _apiClient = apiClient;
            _databaseContext = databaseContext;
        }

        public string GetGithubOauthUrl()
        {
            var clientId = _config["Github:ClientId"];
            var clientSecret = _config["Github:ClientSecret"];
            return $"https://github.com/login/oauth/authorize?client_id={clientId}&client_secret={clientSecret}&scope=user,repo";
        }

        public async Task<GetOauthTokenResult> SetOauthToken(string code, User user)
        {
            try
            {
                var response = await _apiClient.GetOauthToken(code);
                user.GithubUser.AccessToken = response.access_token;
                await _databaseContext.SaveChangesAsync();
                return new GetOauthTokenResult() {Response = response, ErrorMessage = null};
            }
            catch (GithubAPIClientException exception)
            {
                return new GetOauthTokenResult() {Response = null, ErrorMessage = exception.Message};
            }
        }

        public async Task<GetUserInfoResult> SetGithubUserInfo(User user)
        {
            try
            {
                _apiClient.token = user.GithubUser.AccessToken;
                var response = await _apiClient.GetUserInfo();

                user.GithubUser.AvatarUrl = response.avatar_url;
                user.GithubUser.HtmlUrl = response.html_url;
                user.GithubUser.Login = response.login;

                await _databaseContext.SaveChangesAsync();
                return new GetUserInfoResult() {Response = response, ErrorMessage = null};
            }
            catch (GithubAPIClientException exception)
            {
                return new GetUserInfoResult() {Response = null, ErrorMessage = exception.Message};
            }
        }

        public async Task<GetUserRepositoriesResult> GetUserRepositories(User user, int page = 1)
        {
            try
            {
                _apiClient.token = user.GithubUser.AccessToken;
                var response = await _apiClient.GetUserRepositories(page);
                return new GetUserRepositoriesResult() {Response = response, ErrorMessage = null};
            }
            catch (GithubAPIClientException exception)
            {
                return new GetUserRepositoriesResult() {Response = null, ErrorMessage = exception.Message};
            }
        } 
    }
}
