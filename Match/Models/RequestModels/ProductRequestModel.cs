using System.ComponentModel.DataAnnotations;

namespace Match.Models.RequestModels
{
    public class ProductRequestModel
    {
        public string? Description { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
}
