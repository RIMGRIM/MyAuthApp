using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required(ErrorMessage = "帳號是必填的")]
    [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址")]
    [Display(Name = "帳號")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "密碼是必填的")]
    [DataType(DataType.Password)]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "密碼只能包含數字和英文字母")]
    [Display(Name = "密碼")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "確認密碼是必填的")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "密碼與確認密碼不符")]
    [Display(Name = "確認密碼")]
    public required string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "手機號碼為必填")]
    [Phone]
    [StringLength(15, MinimumLength = 10, ErrorMessage = "手機號碼必須介於 10 到 15 位數")]
    [Display(Name = "手機號碼")]
    public string Phone { get; set; }
}
