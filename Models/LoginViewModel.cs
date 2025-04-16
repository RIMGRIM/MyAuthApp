using System.ComponentModel.DataAnnotations;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "帳號")] // 這裡修改顯示名稱
    public required string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "密碼")] // 這裡修改顯示名稱
    public required string Password { get; set; }

    [Display(Name = "記住我")]
    public bool RememberMe { get; set; }
}
