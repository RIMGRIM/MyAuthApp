using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required(ErrorMessage = "帳號是必填的")]
    [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址")]
    [Display(Name = "帳號")]
    public string Email { get; set; }

    [Required(ErrorMessage = "密碼是必填的")]
    [DataType(DataType.Password)]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "密碼只能包含數字和英文字母")]
    [Display(Name = "密碼")]
    public string Password { get; set; }

    [Required(ErrorMessage = "確認密碼是必填的")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "密碼與確認密碼不符")]
    [Display(Name = "確認密碼")]
    public string ConfirmPassword { get; set; }
}
