using System;

namespace PetProject
{
    public class PrivateMessageViewModel
    {
        public Guid MessageId { get; set; } 
        public Guid ChatId { get; set; }    
        public Guid SenderId { get; set; }
        public string Content { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsRead { get; set; }

    }
}
