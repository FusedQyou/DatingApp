using System;

namespace DatingApp.API.DTOs
{
    public class CreateMessage
    {
        public CreateMessage()
        {
            DateSend = DateTime.Now;
        }

        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public DateTime DateSend { get; set; }
        public string Content { get; set; }
    }
}