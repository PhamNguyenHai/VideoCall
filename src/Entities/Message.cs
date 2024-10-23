using System;

namespace PetProject
{
    public class Message
    {
        public Guid MessageId { set; get; }
        public Guid ChatId { set; get; }
        public Guid SenderId { set; get; }
        public Guid ReceiverId { set; get; }
        public string Content { set; get; }
        public DateTime TimeStamp { set; get; }
        public bool? IsDeleted { set; get; }
        public bool? IsRead { set; get; }

    }
}
