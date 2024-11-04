using System;
using System.ComponentModel.DataAnnotations;

namespace PetProject
{
    public class PrivateMessageCreateDto
    {
        [Required(ErrorMessage = "Người gửi không được phép để trống")]
        public Guid SenderId { set; get; }
        public Guid ReceiverId { set; get; }

        [Required(ErrorMessage = "Nội dung tin nhắn không được phép để trống")]
        public string Content { set; get; }
    }
}
