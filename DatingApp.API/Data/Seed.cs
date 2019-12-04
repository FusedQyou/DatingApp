using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        public static void SeedUsers(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            if (!userManager.Users.Any()) {
                var userData = File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);

                var roles = new List<Role> {
                    new Role { Name = "Regular" },
                    new Role { Name = "Moderator" },
                    new Role { Name = "Admin" },
                    new Role { Name = "VIP" }
                };

                foreach(var role in roles) {
                    roleManager.CreateAsync(role).Wait();
                }

                foreach(var user in users)
                {
                    userManager.CreateAsync(user, "password").Wait();
                    userManager.AddToRoleAsync(user, "Regular");
                }

                // Admin user
                var adminUser = new User {
                    UserName = "Admin"
                };

                var result = userManager.CreateAsync(adminUser, "password").Result;
                if (result.Succeeded) {
                    var admin = userManager.FindByNameAsync("Admin").Result;
                    userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator", "Regular" });
                }
            }
        }
    }
}