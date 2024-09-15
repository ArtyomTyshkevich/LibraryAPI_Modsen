using Library.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Library.WebAPI.Setups
{
    public static class DatabaseSetup
    {
        public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<LibraryDbContext>(opt =>
                opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}