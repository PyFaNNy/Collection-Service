using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Course_project.Models
{
    public class UserContext : IdentityDbContext<User, Role , Guid>
    {
        public DbSet<User> User { get; set; }
        public UserContext(DbContextOptions<UserContext> dbContextOptions) : base(dbContextOptions)
        {
          Database.EnsureCreated();
        }
    }
}
