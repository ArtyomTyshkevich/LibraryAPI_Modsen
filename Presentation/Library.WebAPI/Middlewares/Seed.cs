using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
namespace Library.WebAPI.Middlewares
{
    public static class Seed
    {
        public static async Task InitializeRoles(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<long>>>();
            var roles = new[] { "Admin", "User", "Moder" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<long>(role));
                }
            }
        }
    }
}