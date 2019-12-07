using System.ComponentModel.DataAnnotations;

namespace hephaestus.Forms.Core
{
    public class CreateTicketForm
    {
        [Required]
        [MaxLength(128, ErrorMessage="Name is too long")]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
