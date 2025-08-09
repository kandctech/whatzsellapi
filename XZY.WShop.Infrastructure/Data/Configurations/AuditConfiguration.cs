using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XYZ.WShop.Domain;

namespace XZY.WShop.Infrastructure.Data.Configurations
{
    public class AuditConfiguration : IEntityTypeConfiguration<Audit>
    {
        public void Configure(EntityTypeBuilder<Audit> builder)
        {
            builder.HasKey(e => e.Id);
        }
    }
}
