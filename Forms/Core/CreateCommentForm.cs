using System.ComponentModel.DataAnnotations;

namespace hephaestus.Forms.Core
{
    public class CreateCommentForm
    {
        [Required]
        public string Message {get; set;}
    }
}
