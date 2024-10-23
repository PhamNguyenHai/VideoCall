using System;
using System.ComponentModel.DataAnnotations;

namespace PetProject
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Id không được phép để trống")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới không được phép để trống")]
        [StringLength(20, ErrorMessage = "Mật khẩu mới không được phép vượt quá 20 kí tự")]
        public string NewPassword { get; set; }
    }
}
