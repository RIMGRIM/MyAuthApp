using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

public class ApplicationUser : IdentityUser
{
    [Required(ErrorMessage = "手機號碼為必填")]
    [Phone(ErrorMessage = "請輸入有效的手機號碼")]
    [StringLength(15, MinimumLength = 10, ErrorMessage = "手機號碼必須介於 10 到 15 位數")]
    public override string PhoneNumber { get; set; } = string.Empty;



    public string Role { get; set; } // 角色 (可為 "User" 或 "Admin")
    public ApplicationUser()
    {
        // 在建構子中設定 Role 屬性的預設值
        Role = "User"; // 預設角色
    }
}
