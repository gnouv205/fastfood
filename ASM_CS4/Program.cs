using ASM_CS4.Data;
using ASM_CS4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
// Cấu hình dịch vụ Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Thêm dịch vụ vào container.
builder.Services.AddControllersWithViews();

// Cấu hình SqlServer
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnect"))
);

// Cấu hình session và cookie
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian chờ hết hạn
    options.Cookie.HttpOnly = true; // Chỉ truy cập được qua HTTP
    options.Cookie.IsEssential = true; // Đảm bảo cookie cần thiết cho ứng dụng
});

var app = builder.Build();

// Cấu hình pipeline yêu cầu HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Sử dụng session trước các middleware khác
app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
    RequestPath = "/uploads"
});

app.UseRouting();

app.UseAuthorization();

app.MapAreaControllerRoute(
    name: "Admin",
    areaName: "Admin",
    pattern: "Admin/{controller=HomeAdmin}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
