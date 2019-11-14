using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;


namespace hephaestus.Models
{
    public class User : IdentityUser
    {
        public User() : base() {}

        public List<Repository> Repositories {get; set;}

        [InverseProperty("Owner")]
        public List<Project> OwnedProjects {get; set;}

        public List<UserProject> ContributedProjects {get; set;}

        [InverseProperty("User")]
        public List<Invite> Invites {get; set;}
    }
}
