using System;
using System.ComponentModel.DataAnnotations;

namespace Com2usWebProject.ModelResReq
{
    public class PkCreateAccountReq
    {
        [Required]
        [MinLength(1, ErrorMessage = "Email CANNOT BE EMPTY")]
        [StringLength(45, ErrorMessage = "Email IS TOO LONG")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Id is not valid")]
        public String Email { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "PASSWORD CANNOT BE EMPTY")]
        [StringLength(10, ErrorMessage = "PASSWORD IS TOO LONG")]
        [DataType(DataType.Password)]
        public String Password { get; set; }
    }

    public class PkCreateAccountRes
    {
        public CSCommon.ErrorCode Result { get; set; } = CSCommon.ErrorCode.None;
    }
}
