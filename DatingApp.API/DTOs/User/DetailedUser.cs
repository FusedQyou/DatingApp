using System;
using System.Collections.Generic;

namespace DatingApp.API.DTOs
{
    public class DetailedUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string Insertion { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string Introduction { get; set; }
        public string Interests { get; set; }
        public string MainPhotoUrl { get; set; }
        public ICollection<DetailedPhoto> Photos { get; set; }
    }
}