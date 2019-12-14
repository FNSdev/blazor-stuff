using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace hephaestus.Models
{
    public class Project
    {
        public int Id {get; set;}

        [MaxLength(128), Required]
        public string Name {get; set;}

        public string OwnerId {get; set;}
        public User Owner {get; set;}

        public List<UserProject> Contributors {get; set;}

        [InverseProperty("Project")]
        public List<Invite> Invites {get; set;}
                
        public Repository Repository {get; set;}
        
        [InverseProperty("Project")]
        public List<Ticket> Tickets { get; set; }
        
        [InverseProperty("Project")]
        public List<Webhook> Webhooks { get; set; }
    }
}
