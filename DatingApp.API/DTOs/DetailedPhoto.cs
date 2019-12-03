using System;

namespace DatingApp.API.DTOs
{
    public class DetailedPhoto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool AsMainPhoto { get; set; }
    }
}