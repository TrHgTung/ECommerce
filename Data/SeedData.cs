using Microsoft.AspNetCore.Identity;

namespace ECommerce.Data
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roleExists = await roleManager.RoleExistsAsync("Admin");
            if (!roleExists)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var user = await userManager.FindByEmailAsync("admin@example.com");
            if (user == null)
            {
                user = new ApplicationUser { UserName = "admin@example.com", Email = "admin@example.com" };
                await userManager.CreateAsync(user, "Password123!");
            }

            if (!await userManager.IsInRoleAsync(user, "Admin"))
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }

}