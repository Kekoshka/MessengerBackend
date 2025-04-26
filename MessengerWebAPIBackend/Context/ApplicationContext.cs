using MessengerWebAPIBackend.Common;
using MessengerWebAPIBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace MessengerWebAPIBackend.Context
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public ApplicationContext() 
        {
            Database.EnsureCreated();
        }
        public ApplicationContext(DbContextOptions options) : base(options) 
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Config.ConnectionString);
        }
    }
}
