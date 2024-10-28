using System;
using System.ComponentModel.DataAnnotations;
using System.Xml;

namespace PetProject
{
    public class User : BaseAuditEntity, IHasKey
    {
        public Guid UserId {set; get;}

        [Required(ErrorMessage = "Tên người dùng không được phép để trống")]
        [StringLength(100, ErrorMessage = "Tên người dùng không được phép vượt quá 100 kí tự")]
        public string FullName {set; get;}
        public DateTime? DateOfBirth {set; get;}
	    public Gender? Gender {set; get;}

        [Required(ErrorMessage = "Email người dùng không được phép để trống")]
        [StringLength(100, ErrorMessage = "Email người dùng không được phép vượt quá 100 kí tự")]
        [RegularExpression(@"^[a-zA-Z0-9._\\-]+@[a-zA-Z0-9]+(([\\-]*[a-zA-Z0-9]+)*[.][a-zA-Z0-9]+)+(;[ ]*[a-zA-Z0-9._\\-]+@[a-zA-Z0-9]+(([\\-]*[a-zA-Z0-9]+)*[.][a-zA-Z0-9]+)+)*$", ErrorMessage = "Email người dùng không đúng định dạng")]
        public string Email {set; get;}

        [Required(ErrorMessage = "SĐT người dùng không được phép để trống")]
        [StringLength(25, ErrorMessage = "SĐT người dùng không được phép vượt quá 25 kí tự")]
        [RegularExpression(@"^(03|05|07|08|09|01[2|6|8|9])\d{8}$", ErrorMessage = "SĐT người dùng không đúng định dạng")]
        public string PhoneNumber {set; get;}

        [Required(ErrorMessage = "Mật khẩu không được phép để trống")]
        public string Password {set; get;}

        [Required(ErrorMessage = "Quyền người dùng không được phép để trống")]
        public UserRole Role {set; get;}
        public bool? IsBlocked { set; get; } = false;
        public int? WrongPasswordCount { set; get; } = 0;

        public Guid GetKey()
        {
            return UserId;
        }

        public void SetKey(Guid key)
        {
            UserId = key;
        }
    }
}
