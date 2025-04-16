using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyAuthApp.Models;
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager; // 使用 IdentityRole

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager; // 注入 RoleManager
    }

    // 註冊頁面（GET）
    [HttpGet]
    public IActionResult Register() => View();

    // 註冊處理（POST）
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.Phone,
                Role = "User"
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // 確保角色存在（防呆）
                if (!await _roleManager.RoleExistsAsync("User"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("User"));
                }

                // 實際指派角色給帳號（寫進 AspNetUserRoles）
                await _userManager.AddToRoleAsync(user, "User");

                TempData["SuccessMessage"] = "註冊成功!請以此帳號密碼登入";
                return RedirectToAction("Login"); // 導回 Login 頁
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(model);
    }

    // 登入頁面（GET）
    [HttpGet]
    public IActionResult Login() => View();

    // 登入處理（POST）
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "無效的登入資訊");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "登入失敗");
            return View(model);
        }

        // 判斷角色並導向不同的頁面
        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            return Redirect("/Management/User/Index");
        }
        else
        {
            return RedirectToAction("Index", "Home");
        }
        
    }

    // 登出
    [HttpPost]
    [ValidateAntiForgeryToken] // 防止 CSRF 攻擊
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        HttpContext.Session.Clear(); // 強制清除 Session
        return RedirectToAction("Login", "Account");
    }

    // 指派角色
    public async Task<IActionResult> AssignRole(string userEmail, string role)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            return NotFound("使用者不存在");
        }

        if (!await _userManager.IsInRoleAsync(user, role))
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        return Ok($"已將 {userEmail} 設為 {role} 角色");
    }
}
