using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechNews.Core.Api.Data.Models;

namespace TechNews.Core.Api.Data.ModelConfigurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.ToTable("Authors");
        builder.HasKey(p => p.Id).HasName("PK_Authors");
        builder.Property(p => p.Id).HasColumnType("UNIQUEIDENTIFIER").HasColumnName("Id").IsRequired();
        builder.Property(p => p.CreatedAt).HasColumnType("DATETIME").HasColumnName("CreatedAt").IsRequired();
        builder.Property(p => p.IsDeleted).HasColumnType("BIT").HasColumnName("IsDeleted").IsRequired();
        builder.Property(p => p.Name).HasColumnType("VARCHAR(100)").HasColumnName("Name").IsRequired();
        builder.Property(p => p.Email).HasColumnType("VARCHAR(100)").HasColumnName("Email").IsRequired();
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
