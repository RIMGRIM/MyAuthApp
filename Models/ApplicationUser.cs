using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

public class ApplicationUser : IdentityUser
{
    [Required]
    [Phone]
    [StringLength(15, MinimumLength = 10, ErrorMessage = "手機號碼必須介於 10 到 15 位數")]
    public override required string PhoneNumber { get; set; }


    public required string Role { get; set; } // 角色 (可為 "User" 或 "Admin")
}
