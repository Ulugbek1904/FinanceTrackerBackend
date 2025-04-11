using FinanceTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceTracker.Infrastructure.Brokers.Storages.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasMany(c => c.Transactions)
                .WithOne(t => t.Category)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasData(
                new Category { Id = 1, Name = "Salary", IsIncome = true, IsDefault = true, CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) },
                new Category { Id = 2, Name = "Bonus", IsIncome = true, IsDefault = true, CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) },
                new Category { Id = 3, Name = "Investment", IsIncome = true, IsDefault = true, CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) },
                new Category { Id = 4, Name = "Food", IsIncome = false, IsDefault = true, CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) },
                new Category { Id = 5, Name = "Transportation", IsIncome = false, IsDefault = true, CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) },
                new Category { Id = 6, Name = "Bills", IsIncome = false, IsDefault = true, CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) },
                new Category { Id = 7, Name = "Shopping", IsIncome = false, IsDefault = true, CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) },
                new Category { Id = 8, Name = "Entertainment", IsIncome = false, IsDefault = true, CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) },
                new Category { Id = 9, Name = "Health", IsIncome = false, IsDefault = true, CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) },
                new Category { Id = 10, Name = "Rent", IsIncome = false, IsDefault = true, CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) },
                new Category { Id = 11, Name = "Utilities", IsIncome = false, IsDefault = true, CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) },
                new Category { Id = 12, Name = "Other", IsIncome = false, IsDefault = true, CreatedAt = new DateTime(2025, 1, 1), UpdatedAt = new DateTime(2025, 1, 1) });

        }
    }
}
