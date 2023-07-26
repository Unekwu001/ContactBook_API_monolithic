using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ContactBook_API.Models
{
    public class TheoDbContext : IdentityDbContext<User>
    {
        public TheoDbContext(DbContextOptions<TheoDbContext> options) : base(options)
        {

        }
       
    }
}
