using FinanceTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceTracker.Infrastructure.Brokers.Storages.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Name).IsRequired().HasMaxLength(100);
            builder.Property(a => a.Type).HasConversion<string>();
        }
    }
}
