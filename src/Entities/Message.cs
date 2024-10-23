using PetProject.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace PetProject
{
    public class Message : IHasKey
    {
        public Guid MessageId { set; get; }
        public Guid ChatId { set; get; }
        public Guid SenderId { set; get; }
        public Guid ReceiverId { set; get; }

        [Required(ErrorMessage = "Nội dung tin nhắn không được phép để trống")]
        public string Content { set; get; }

        [Required(ErrorMessage = "Thời gian gửi tin nhắn không được phép để trống")]
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
