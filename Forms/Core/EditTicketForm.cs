using System.ComponentModel.DataAnnotations;
using hephaestus.Models;

namespace hephaestus.Forms.Core
{
    public class EditTicketForm
    {
        [Required]
        [MaxLength(128, ErrorMessage="Name is too long")]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
        
        [Required]
        public Ticket.TicketStatus Status { get; set; }
    }
}
