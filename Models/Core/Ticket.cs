using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hephaestus.Models
{
    public class Ticket
    {
        public enum TicketStatus
        {
            Pending,
            InProgress,
            InReview,
            ReadyForQA,
            Blocked,
            Done,
        }
        public int Id { get; set; }

        [MaxLength(128), Required]
        public string Name { get; set; }
        
        [Required]
        public string Description { get; set; }

        public List<UserTicket> Assignees {get; set;}
        
        public TicketStatus Status { get; set; }
        
        public int ProjectId { get; set; }
        
        public Project Project { get; set; }
        
        [InverseProperty("Ticket")]
        public List<Comment> Comments { get; set; }
    }
}
