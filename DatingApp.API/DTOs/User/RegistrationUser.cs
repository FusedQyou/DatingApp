using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTOs
{
    public class RegistrationUser
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(int.MaxValue, MinimumLength = 4, ErrorMessage = "Your password must be atleast 4 characters.")]
        public string Password { get; set; }
    }
}