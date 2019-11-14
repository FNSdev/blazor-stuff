using System.ComponentModel.DataAnnotations;


namespace hephaestus.Models
{
    public class Repository
    {
        public int Id {get; set;}
        
        [MaxLength(128), Required]
        public string Name {get; set;}

        public string UserId {get; set;}
        public User Owner {get; set;}
    }
}
