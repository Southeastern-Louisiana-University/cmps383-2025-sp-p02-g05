using Microsoft.AspNetCore.Identity;
using Selu383.SP25.P02.Api.Features.Roles;
using Selu383.SP25.P02.Api.Features.Users;

namespace Selu383.SP25.P02.Api.Data
{
    public static class SeedUsers
    {
        public static async Task Initialize(UserManager<User> userManager, RoleManager<Role> roleManager)
        {

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new Role { Name = "Admin" });
            }
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new Role { Name = "User" });
            }

            //admin
            if (await userManager.FindByNameAsync("galkadi") == null)
            {
                var adminUser = new User { UserName = "galkadi" };
                await userManager.CreateAsync(adminUser, "Password123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            //users
            if (await userManager.FindByNameAsync("bob") == null)
            {
                var userBob = new User { UserName = "bob" };
                await userManager.CreateAsync(userBob, "Password123!");
                await userManager.AddToRoleAsync(userBob, "User");
            }

            if (await userManager.FindByNameAsync("sue") == null)
            {
                var userSue = new User { UserName = "sue" };
                await userManager.CreateAsync(userSue, "Password123!");
                await userManager.AddToRoleAsync(userSue, "User");
            }
        }
    }
}