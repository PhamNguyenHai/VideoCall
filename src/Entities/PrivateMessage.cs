using System;
using System.ComponentModel.DataAnnotations;

namespace PetProject
{
    public class PrivateMessage : IHasKey
    {
        public Guid MessageId { set; get; }
        public Guid ChatId { set; get; }
        public Guid SenderId { set; get; }

        [Required(ErrorMessage = "Nội dung tin nhắn không được phép để trống")]
        public string Content { set; get; }

        public DateTime TimeStamp { set; get; }
        public bool? IsDeleted { set; get; }
        public bool? IsRead { set; get; }

        public Guid GetKey()
        {
            return MessageId;
        }

        public void SetKey(Guid key)
        {
            MessageId = key;
        }
    }
}
