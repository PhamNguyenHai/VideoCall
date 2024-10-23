using System;
using System.ComponentModel.DataAnnotations;

namespace PetProject
{
    public class BaseAuditEntity
    {
        [Required(ErrorMessage = "Ngày tạo không được phép để trống")]
        public DateTime CreatedDate { set; get; }

        [Required(ErrorMessage = "Ngày cập nhật không được phép để trống")]
        public DateTime ModifiedDate { set; get; }
    }
}
