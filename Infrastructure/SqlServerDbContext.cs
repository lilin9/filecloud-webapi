using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure {
    public class SqlServerDbContext : DbContext {

        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<Files> Files { get; set; }

        public SqlServerDbContext() { }

        public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
