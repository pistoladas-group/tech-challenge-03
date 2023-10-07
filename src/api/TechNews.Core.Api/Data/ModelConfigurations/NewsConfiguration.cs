using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechNews.Core.Api.Data.Models;

namespace TechNews.Core.Api.Data.ModelConfigurations;

public class NewsConfiguration : IEntityTypeConfiguration<News>
{
    public void Configure(EntityTypeBuilder<News> builder)
    {
        builder.ToTable("News");
        builder.HasOne(p => p.Author).WithMany(p => p.News).HasForeignKey(p => p.AuthorId).HasConstraintName("FK_News_Authors");
        builder.HasKey(p => p.Id).HasName("PK_News");
        builder.Property(p => p.Id).HasColumnType("UNIQUEIDENTIFIER").HasColumnName("Id").IsRequired();
        builder.Property(p => p.CreatedAt).HasColumnType("DATETIME").HasColumnName("CreatedAt").IsRequired();
        builder.Property(p => p.IsDeleted).HasColumnType("BIT").HasColumnName("IsDeleted").IsRequired();
        builder.Property(p => p.Title).HasColumnType("VARCHAR(100)").HasColumnName("Title").IsRequired();
        builder.Property(p => p.Description).HasColumnType("VARCHAR(5000)").HasColumnName("Description").IsRequired();
        builder.Property(p => p.PublishDate).HasColumnType("DATETIME").HasColumnName("PublishDate").IsRequired();
        builder.Property(p => p.AuthorId).HasColumnType("UNIQUEIDENTIFIER").HasColumnName("AuthorId").IsRequired();
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
