using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 加入資料庫連線設定
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 加入 Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    // 設定密碼規則，去除強制大小寫和符號
    options.Password.RequireDigit = true;          // 要求數字
    options.Password.RequireLowercase = false;    // 不強制小寫字母
    options.Password.RequireUppercase = false;    // 不強制大寫字母
    options.Password.RequireNonAlphanumeric = false; // 不強制符號
    options.Password.RequiredLength = 6;          // 密碼最少長度
    options.Password.RequiredUniqueChars = 1;     // 必須有至少一個不同字母
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// 設定 Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// 確保身份驗證啟用
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
