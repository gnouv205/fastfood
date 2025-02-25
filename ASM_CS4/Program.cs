using ASM_CS4.Data;
using ASM_CS4.Models;
using ASM_CS4.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Identity.UI.Services;


var builder = WebApplication.CreateBuilder(args);

// Cấu hình kết nối đến cơ sở dữ liệu SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnect"))
);

builder.Services.AddHttpClient();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.AddScoped<IEmailSender, EmailSender>();



// Cấu hình Identity với Customer làm User
builder.Services.AddIdentity<Customer, IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddDefaultTokenProviders();

// Cấu hình session và cookie
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

// Cấu hình Cookie Authentication
builder.Services.ConfigureApplicationCookie(options =>
{
	options.Cookie.HttpOnly = true;  // ✅ Đảm bảo Cookie được trình duyệt lưu
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // ✅ Đảm bảo hoạt động trên HTTPS
	options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // ✅ Cookie tồn tại 60 phút
	options.SlidingExpiration = true;  // ✅ Gia hạn nếu người dùng hoạt động
	options.LoginPath = "/Account/Login"; // ✅ Trang login
	options.LogoutPath = "/Account/Logout"; // ✅ Trang logout
	options.AccessDeniedPath = "/Account/AccessDenied"; // ✅ Trang từ chối quyền truy cập
});
builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));  // ✅ Policy yêu cầu role Admin
});


#region Login Google
// Cấu hình đăng nhập bằng Google
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["GoogleKeys:ClientId"];
        options.ClientSecret = builder.Configuration["GoogleKeys:ClientSecret"];
    });

#endregion



// Cấu hình dịch vụ Localization (đa ngôn ngữ)
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");



// Thêm MVC (Controllers và Views)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Cấu hình pipeline xử lý HTTP
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

// Sử dụng session trước các middleware khác
app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

// Hỗ trợ tải file từ thư mục wwwroot/uploads
app.UseStaticFiles(new StaticFileOptions
{
	FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
	RequestPath = "/uploads"
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


// Cấu hình route cho Admin
app.MapAreaControllerRoute(
	name: "Admin",
	areaName: "Admin",
	pattern: "Admin/{controller=HomeAdmin}/{action=Index}/{id?}"
);

// Cấu hình route mặc định
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();

