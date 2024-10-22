using System;

namespace PetProject.Models
{
    public class Message
    {
        public User Sender { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
    }
}
