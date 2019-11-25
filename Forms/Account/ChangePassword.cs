using System.ComponentModel.DataAnnotations;

namespace hephaestus.Forms.Account 
{
    public class ChangePasswordForm
    {
        [Required]
        public string OldPassword {get; set;}
        [Required]
        public string NewPassword {get; set;}
    }
}
