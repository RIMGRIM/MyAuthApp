using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuthApp.Areas.Management.Controllers
{
    [Area("Management")]
    [Authorize(Roles = "Admin")]  // 限制只有 Admin 角色可以存取
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager; // 加入 RoleManager

        // 更新建構函式，加入 RoleManager
        public UserController(UserManager<ApplicationUser> userManager, 
                              SignInManager<ApplicationUser> signInManager,
                              RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager; // 設置 RoleManager
        }

        // **顯示所有帳號 (分頁)**
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var users = await _userManager.Users
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            int totalUsers = await _userManager.Users.CountAsync();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalUsers / pageSize);
            ViewBag.CurrentPage = page;

            return View(users);
        }

        // **測試當前登入者是否為 Admin**
        [Authorize]
        public IActionResult TestRole()
        {
            var isAdmin = User.IsInRole("Admin");
            return Content($"Is Admin: {isAdmin}");
        }

        // **查看帳號詳細資訊**
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        // **編輯帳號 (GET)**
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "找不到使用者";
                return RedirectToAction("Index");
            }

            // 如果 Role 屬性沒有值，從 Identity Roles 裡抓一份給它
            if (string.IsNullOrEmpty(user.Role))
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.Role = roles.FirstOrDefault() ?? "User";
            }

            return View(user);
        }

        // **編輯帳號 (POST)**
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUser updatedUser)
        {
            if (id != updatedUser.Id)
            {
                TempData["ErrorMessage"] = "請求無效，ID 不匹配";
                return RedirectToAction("Index");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "找不到使用者";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["ErrorMessage"] = "輸入資料有誤：" + string.Join(", ", errors);
                return View(updatedUser);
            }

            // ⚡ 更新資料表內的欄位
            user.Email = updatedUser.Email;
            user.PhoneNumber = updatedUser.PhoneNumber;
            user.Role = updatedUser.Role;

            // ⚡ 防止自己修改自己的 Admin 權限
            if (User.Identity.Name == user.UserName && updatedUser.Role != "Admin")
            {
                TempData["ErrorMessage"] = "不能修改自己的管理員角色！";
                return RedirectToAction("Index");
            }

            // ⚡ Identity 系統的角色同步
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(updatedUser.Role))
            {
                // 移除舊角色
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

                // 確保新角色存在，不存在就新增
                if (!await _roleManager.RoleExistsAsync(updatedUser.Role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(updatedUser.Role));
                }

                // 加入新角色
                await _userManager.AddToRoleAsync(user, updatedUser.Role);
            }

            // ⚡ 更新資料表
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "帳號更新成功！";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "編輯失敗：" + string.Join(", ", result.Errors.Select(e => e.Description));
            return View(updatedUser);
        }


        // **刪除帳號**
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "找不到使用者";
                return RedirectToAction("Index");
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any())
            {
                var roleResult = await _userManager.RemoveFromRolesAsync(user, roles);
                if (!roleResult.Succeeded)
                {
                    TempData["ErrorMessage"] = "無法移除角色：" + string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    return RedirectToAction("Index");
                }
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "帳號已成功刪除";
            }
            else
            {
                TempData["ErrorMessage"] = "刪除失敗：" + string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction("Index");
        }

        // **登出**
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
             Console.WriteLine(">>> 登出被呼叫了 <<<");

            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            Response.Cookies.Delete(".AspNetCore.Identity.Application");
            return RedirectToAction("Login", "Account", new { area = "" });
        }
    }
}
