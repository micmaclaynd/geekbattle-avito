using AnalyticsMicroservice.Models;
using Microsoft.EntityFrameworkCore;

namespace AnalyticsMicroservice.Contexts {
    public class ApplicationContext : DbContext {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<CategoryModel>().HasData(new CategoryModel() {
                Id = 1,
                Name = "",
                ParentId = null
            });
            modelBuilder.Entity<LocationModel>().HasData(new LocationModel() {
                Id = 1,
                Name = "",
                ParentId = null
            });
        }

        public DbSet<PriceModel> Prices { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<LocationModel> Locations { get; set; }
        public DbSet<MatrixModel> Matrices { get; set; }
    }
}
