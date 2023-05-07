using BookShoppingCartMvcUI.Constants;
using Microsoft.AspNetCore.Identity;
using Nine.Models;

namespace Nine.Data
{
    public class DbSeeder
    {
        public static async Task SeedDefaultData(IServiceProvider service)
        {
            //Seed Role
            var userManager = service.GetService<UserManager<ApplicationUser>>();
            var roleManager = service.GetService<RoleManager<IdentityRole>>();
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Owner.ToString()));


            var user = new ApplicationUser
            {
                UserName = "User@gmail.com",
                Email = "User@gmail.com",
                Descripstion = "User",
                EmailConfirmed = true

            };

            var userInDb = await userManager.FindByEmailAsync(user.Email);
            if (userInDb == null)
            {
                await userManager.CreateAsync(user, "User@123");
                await userManager.AddToRoleAsync(user, Roles.User.ToString());
            }

            var owner = new ApplicationUser
            {
                UserName = "Owner@gmail.com",
                Email = "Owner@gmail.com",
                Descripstion = "Owner",
                EmailConfirmed = true

            };

            var ownerInDb = await userManager.FindByEmailAsync(owner.Email);
            if (userInDb == null)
            {
                await userManager.CreateAsync(owner, "Owner@123");
                await userManager.AddToRoleAsync(owner, Roles.Owner.ToString());
            }

            var admin = new ApplicationUser
            {
                UserName = "Admin@gmail.com",
                Email = "Admin@gmail.com",
                Descripstion = "Admin",
                EmailConfirmed = true

            };

            var adminInDb = await userManager.FindByEmailAsync(admin.Email);
            if (userInDb == null)
            {
                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, Roles.Admin.ToString());
            }
        }
    }
}
