using Microsoft.EntityFrameworkCore;
using UsersMicroservice.Models;

namespace UsersMicroservice.Contexts {
    public class ApplicationContext : DbContext {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        public DbSet<UserModel> Users { get; set; }
    }
}