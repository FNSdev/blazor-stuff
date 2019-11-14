namespace hephaestus.Models
{
    public class Invite
    {
        public int Id {get; set;}
        public string Message {get; set;}

        public string UserId {get; set;}
        public User User {get; set;}

        public int ProjectId {get; set;}
        public Project Project {get; set;}
    }
}
