using System;
using DatingApp.API.Models;

namespace DatingApp.API.DTOs
{
    public class ReturnedMessage
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public ListUser Sender { get; set; }
        public int RecipientId { get; set; }
        public ListUser Recipient { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime DateSend { get; set; }
        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }
    }
}