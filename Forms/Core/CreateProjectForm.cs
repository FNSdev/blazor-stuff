using System.ComponentModel.DataAnnotations;


namespace hephaestus.Forms.Core 
{
    public class CreateProjectForm
    {
        [Required]
        [MaxLength(256, ErrorMessage="Name is too long")]
        public string Name {get; set;}
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage="Repository must be selected")]
        public int RepositoryId {get; set;}
    }
}
