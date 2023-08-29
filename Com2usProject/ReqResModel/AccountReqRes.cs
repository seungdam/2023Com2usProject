using Com2usProject.DataModel;
using System.ComponentModel.DataAnnotations;

namespace Com2usProject.ReqResModel;

public class AccountReq
{

    [Required]
    [MinLength(1, ErrorMessage = "EMAIL CANNOT BE EMPTY")]
    [StringLength(50, ErrorMessage = "EMAIL IS TOO LONG")]
    [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
    [DataType(DataType.EmailAddress)]
    public String Email { get; set; }
 
    [Required]
    [MinLength(1, ErrorMessage = "PASSWORD CANNOT BE EMPTY")]
    [StringLength(10, ErrorMessage = "PASSWORD IS TOO LONG")]
    [DataType(DataType.Password)]
    public String Password { get; set; }
}

public class RegisterAccountRes
{
    public CSCommon.ErrorCode ErrorCode { get; set; }
}

public class LoginAccountRes
{

    public String AuthToken { get; set; }

   public PlayableCharacterStatusInfo[]? PlayerInfos { get; set; }
    public CSCommon.ErrorCode ErrorCode { get; set; }

}