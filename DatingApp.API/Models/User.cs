using System;
using System.Collections.Generic;

namespace DatingApp.API.Models
{
    public class User
    {
        // Identification
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        // Personal
        public string FirstName { get; set; }
        public string Insertion { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string Introduction { get; set; }
        public string Interests { get; set; }
        public ICollection<Photo> Photos { get; set; }
    }
}