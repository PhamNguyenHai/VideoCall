using System.ComponentModel.DataAnnotations;

namespace PetProject
{
    public class LoginInfor
    {
        [Required(ErrorMessage = "Email/SĐT không được phép để trống")]
        [StringLength(100, ErrorMessage = "Email/SĐT không được phép vượt quá 100 kí tự")]
        public string EmailOrPhoneNumber { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được phép để trống")]
        [StringLength(20, ErrorMessage = "Mật khẩu không được phép vượt quá 20 kí tự")]
        public string Password { get; set; }
    }
}
