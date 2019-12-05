using System.ComponentModel.DataAnnotations;


namespace hephaestus.Models
{
    public class UserProject
    {
        public string ContributorId { get; set; }
        public User Contributor { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }    
    }
}
