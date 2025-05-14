using Microsoft.EntityFrameworkCore;
using UserManagement.Models;

namespace UserManagement.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) 
        { 

        }

        public DbSet<User> User { get; set; }
        public DbSet<UserGroup> UserGroup { get; set; }
    }
}
