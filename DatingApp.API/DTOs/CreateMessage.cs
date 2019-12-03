using System;

namespace DatingApp.API.DTOs
{
    public class CreateMessage
    {
        public CreateMessage()
        {
            MessageSent = DateTime.Now;
        }

        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public DateTime MessageSent { get; set; }
        public string Content { get; set; }
    }
}