using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Uranus.AuthApi.Models;


namespace Uranus.AuthApi.Data
{
    //public class ApplicationDbContext : IdentityDbContext<IdentityUser, ApplicationRole, string>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<AvailableClaim> AvailableClaims { get; set; }
    }
}
