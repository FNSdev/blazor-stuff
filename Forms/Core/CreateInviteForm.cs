using System.ComponentModel.DataAnnotations;


namespace hephaestus.Forms.Core 
{
    public class CreateInviteForm
    {
        [Required]
        [MaxLength(256, ErrorMessage="UserName is too long")]
        public string UserName {get; set;}
        
        [Required]
        public string Message {get; set;}
    }
}
