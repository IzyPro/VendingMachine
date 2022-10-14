using System.ComponentModel.DataAnnotations;

namespace Match.Models.DTOs
{
    public class LoginResponseDTO
    {
        [Required]
        public string? UserId { get; set; }
        public string? token { get; set; }
        [Required]
        public decimal AccountBalance { get; set; }
        [Required]
        public string? email { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
    }
}
