using System.ComponentModel.DataAnnotations;


namespace hephaestus.Forms.Account 
{
    public class RegisterForm
    {
        [Required]
        [MaxLength(256, ErrorMessage="UserName is too long")]
        public string UserName {get; set;}
        [Required]
        [MaxLength(256, ErrorMessage="Email is too long")]
        public string Email {get; set;}
        [Required]
        public string Password {get; set;}
    }
}