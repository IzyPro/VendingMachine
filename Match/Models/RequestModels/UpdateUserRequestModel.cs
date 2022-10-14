using Match.Utilities;
using System.ComponentModel.DataAnnotations;

namespace Match.Models.RequestModels
{
    public class UpdateUserRequestModel
    {
        [Required]
        public string? UserId { get; set; }
        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }
    }
}
