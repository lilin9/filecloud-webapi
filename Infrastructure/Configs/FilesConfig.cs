
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configs {
    internal class FilesConfig: IEntityTypeConfiguration<Files> {
        public void Configure(EntityTypeBuilder<Files> builder) {
            builder.ToTable("Files");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.FileId).IsRequired();
            builder.Property(x => x.FileName).IsRequired().HasMaxLength(1024);
            builder.Ignore(x => x.StaticDownloadUrl);
            builder.Ignore(x => x.DynamicDownloadUrl);
            builder.OwnsOne(x => x.FileSize).Property(x => x.Unit).HasConversion<string>().HasMaxLength(20);
            builder.Property(x => x.FileMimeType).HasMaxLength(128);
            builder.Property(x => x.FileOnlyTag).HasMaxLength(128);
        }
    }
}
