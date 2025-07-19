using ECommerce.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn phiên làm việc
    options.Cookie.HttpOnly = true; // Bảo vệ cookie khỏi truy cập từ JavaScript
    options.Cookie.IsEssential = true; // Cookie cần thiết cho ứng dụng
});
// Cấu hình MySQL
builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 21))) // Phiên bản MySQL, thay đổi nếu cần
);
// Cấu hình Identity (Quản lý người dùng)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});


static async Task SeedRolesAndAdmin(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roleNames = { "Admin", "User" };

    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Tạo user admin nếu chưa tồn tại
    var adminEmail = "4601104211@student.hcmue.edu.vn";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FirstName = "Admin",              // 👈 Bổ sung FirstName
            LastName = "System",              // 👈 Bổ sung LastName
            DateOfBirth = DateTime.UtcNow     // 👈 Bổ sung nếu DB yêu cầu
        };

        var result = await userManager.CreateAsync(admin, "Admin123$");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"Error creating admin user: {error.Description}");
            }
        }
    }
}


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await SeedRolesAndAdmin(scope.ServiceProvider);
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// using (var scope = app.Services.CreateScope())
// {
//     var services = scope.ServiceProvider;
//     var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
//     SeedData.Initialize(services, userManager).Wait();
// }

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
