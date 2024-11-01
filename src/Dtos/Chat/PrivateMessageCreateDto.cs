using System;
using System.ComponentModel.DataAnnotations;

namespace PetProject
{
    public class PrivateMessageCreateDto
    {
        public Guid ChatId { set; get; }
        [Required(ErrorMessage = "Người gửi không được phép để trống")]
        public Guid SenderId { set; get; }

        [Required(ErrorMessage = "Nội dung tin nhắn không được phép để trống")]
        public string Content { set; get; }
    }
}
