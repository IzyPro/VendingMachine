using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Match.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        public decimal AccountBalance { get; set; }
        [Required]
        public DateTime DateCreated { get; set; }
        [Required]
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
