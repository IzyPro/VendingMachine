using System.ComponentModel.DataAnnotations;

namespace Match.Models
{
    public class Product : BaseModel
    {
        public string? Description { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
}
