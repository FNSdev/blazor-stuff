using System.ComponentModel.DataAnnotations;


namespace hephaestus.Forms.Account 
{
    public class LoginForm
    {
        [Required]
        public string UserName {get; set;}
        [Required]
        public string Password {get; set;}
    }
}
