using System;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTOs
{
    public class RegistrationUser
    {
        public RegistrationUser()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }

        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(int.MaxValue, MinimumLength = 4, ErrorMessage = "Your password must be atleast 4 characters.")]
        public string Password { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string FirstName { get; set; }
        public string Insertion { get; set; }
        public string LastName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
    }
}