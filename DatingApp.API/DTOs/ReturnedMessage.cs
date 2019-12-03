using System;
using DatingApp.API.Models;

namespace DatingApp.API.DTOs
{
    public class ReturnedMessage
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderFirstName { get; set; }
        public string SenderInsertion { get; set; }
        public string SenderLastName { get; set; }
        public string SenderMainPhotoUrl { get; set; }
        public int RecipientId { get; set; }
        public string RecipientFirstName { get; set; }
        public string RecipientInsertion { get; set; }
        public string RecipientLastName { get; set; }
        public string RecipientMainPhotoUrl { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime DateSend { get; set; }
    }
}