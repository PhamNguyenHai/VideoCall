using System;
using System.ComponentModel.DataAnnotations;

namespace PetProject
{
    public class UserCreateDto
    {
        [Required(ErrorMessage = "Tên người dùng không được phép để trống")]
        [StringLength(100, ErrorMessage = "Tên người dùng không được phép vượt quá 100 kí tự")]
        public string FullName { set; get; }
        public DateTime? DateOfBirth { set; get; }
        public Gender? Gender { set; get; }

        [Required(ErrorMessage = "Email người dùng không được phép để trống")]
        [StringLength(100, ErrorMessage = "Email người dùng không được phép vượt quá 100 kí tự")]
        [RegularExpression(@"^[a-zA-Z0-9._\\-]+@[a-zA-Z0-9]+(([\\-]*[a-zA-Z0-9]+)*[.][a-zA-Z0-9]+)+(;[ ]*[a-zA-Z0-9._\\-]+@[a-zA-Z0-9]+(([\\-]*[a-zA-Z0-9]+)*[.][a-zA-Z0-9]+)+)*$", ErrorMessage = "Email người dùng không đúng định dạng")]
        public string Email { set; get; }

        [Required(ErrorMessage = "SĐT người dùng không được phép để trống")]
        [StringLength(25, ErrorMessage = "SĐT người dùng không được phép vượt quá 25 kí tự")]
        [RegularExpression(@"^(03|05|07|08|09|01[2|6|8|9])\d{8}$", ErrorMessage = "SĐT người dùng không đúng định dạng")]
        public string PhoneNumber { set; get; }

        [Required(ErrorMessage = "Mật khẩu không được phép để trống")]
        public string Password { set; get; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }

        public UserRole Role { get; set; }
    }
}
