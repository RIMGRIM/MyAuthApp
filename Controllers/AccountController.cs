using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyAuthApp.Models;
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
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
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "註冊成功!請以此帳號密碼登入";
                return View(); // 返回 Register 頁面讓 JavaScript 處理跳轉
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
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "登入成功!";
                return View(); // 返回 Login 頁面讓 JavaScript 處理跳轉
            }
            ModelState.AddModelError("", "登入失敗，請檢查帳號或密碼。");
        }
        return View(model);
    }

    // 登出
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }
}
