using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Course_project.Models
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<User> Users { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> dbContextOptions) : base(dbContextOptions)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
