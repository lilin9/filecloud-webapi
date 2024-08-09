using Domain.Attributes;
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


        public override int SaveChanges() {
            //设置更新事件
            SetUpdateTime();
            return base.SaveChanges();
        }

        /// <summary>
        /// 统一设置实体的更新时间
        /// </summary>
        private void SetUpdateTime() {
            //获取所有被修改的实体
            var modifiedEntity = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified).Select(e => e.Entity)
                .ToList();
            modifiedEntity.ForEach(entity => {
                //获取所有被标注了 HasUpdateTimeAttribute 特性，并且是 DateTime 类型的属性
                var properties = entity.GetType().GetProperties()
                    .Where(p => p.IsDefined(typeof(HasUpdateTimeAttribute), false) &&
                                p.PropertyType == typeof(DateTime)).ToList();
                //将其值更新为当前事件
                properties.ForEach(p => p.SetValue(entity, DateTime.Now));
            });
        }
    }
}
