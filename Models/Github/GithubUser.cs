namespace hephaestus.Models
{
    public class GithubUser
    {
        public int Id {get; set;}
        public string AccessToken {get; set;}
        public string Login {get; set;}
        public string AvatarUrl {get; set;}
        public string HtmlUrl {get; set;}
        public string UserId {get; set;}
        public User User {get; set;}
    }
}
