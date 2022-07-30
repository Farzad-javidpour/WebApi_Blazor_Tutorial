using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApi.Infrastructure.ModelConfigurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<WebApi.Core.Domain.Product>
    {
        public void Configure(EntityTypeBuilder<WebApi.Core.Domain.Product> builder)
        {
            builder.Property(a => a.Title).IsRequired().HasMaxLength(50);
            builder.HasIndex(a => a.Title).IsUnique();
        }
    }
}
