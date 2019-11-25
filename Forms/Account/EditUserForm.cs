using System.ComponentModel.DataAnnotations;


namespace hephaestus.Forms.Account 
{
    public class EditUserForm
    {
        [Required]
        [MaxLength(256, ErrorMessage="UserName is too long")]
        public string UserName {get; set;}
        [Required]
        [MaxLength(256, ErrorMessage="Email is too long")]
        public string Email {get; set;}
    }
}
