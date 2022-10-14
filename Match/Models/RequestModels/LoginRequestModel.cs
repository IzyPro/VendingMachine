using System.ComponentModel.DataAnnotations;

namespace Match.Models.RequestModels
{
    public class LoginRequestModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
