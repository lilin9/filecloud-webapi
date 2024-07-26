using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configs {
    internal class UserInfoConfig: IEntityTypeConfiguration<UserInfo> {
        public void Configure(EntityTypeBuilder<UserInfo> builder) {
            builder.ToTable("UserInfos");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.CreateTime).HasField("createTime");
            builder.Property(x => x.Avatar).HasMaxLength(128);
            builder.OwnsOne(x => x.BanTime).Property(x => x.Unit).HasConversion<string>();
            builder.Property(x => x.Email).IsRequired().HasMaxLength(128);
            builder.Property(x => x.UserName).IsRequired().HasMaxLength(128);
            builder.Property(x => x.Password).IsRequired().HasMaxLength(128);
            builder.Property(x => x.DisableReason).HasMaxLength(128);
            builder.OwnsOne(x => x.UserDisk, ud => {
                ud.OwnsOne(x => x.TotalCapacity).Property(x => x.Unit).HasConversion<string>();
                ud.OwnsOne(x => x.UsedCapacity).Property(x => x.Unit).HasConversion<string>();
                ud.OwnsOne(x => x.RemainingCapacity).Property(x => x.Unit).HasConversion<string>();
            });
        }
    }
}
