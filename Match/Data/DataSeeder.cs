using Match.Utilities;
using Microsoft.AspNetCore.Identity;

namespace Match.Data
{
    public static class DataSeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(UserRoleEnum.Seller.ToString()));
            await roleManager.CreateAsync(new IdentityRole(UserRoleEnum.Buyer.ToString()));
        }
    }
}
