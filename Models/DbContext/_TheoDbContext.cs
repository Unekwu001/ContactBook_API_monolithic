using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ContactBook_API.Models.DbContext
{
    public class TheoDbContext : IdentityDbContext<User>
    {
        public TheoDbContext(DbContextOptions<TheoDbContext> options) : base(options)
        {

        }

    }
}
