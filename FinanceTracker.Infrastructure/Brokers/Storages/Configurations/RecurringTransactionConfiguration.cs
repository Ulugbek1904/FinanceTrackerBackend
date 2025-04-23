using FinanceTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceTracker.Infrastructure.Brokers.Storages.Configurations
{
    public class RecurringTransactionConfiguration : IEntityTypeConfiguration<RecurringTransaction>
    {
        public void Configure(EntityTypeBuilder<RecurringTransaction> builder)
        {
            builder.HasKey(rt => rt.Id);
            builder.Property(rt => rt.Amount).HasColumnType("decimal(18,2)");
            builder.Property(rt => rt.Description).IsRequired();
            builder.Property(rt => rt.RecurrenceType).HasConversion<string>();
            builder.Property(rt => rt.IsIncome).IsRequired();
            builder.HasOne(rt => rt.Account)
                .WithMany()
                .HasForeignKey(rt => rt.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(rt => rt.Category)
                .WithMany()
                .HasForeignKey(rt => rt.CategoryId);
        }
    }
}
