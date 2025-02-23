using FinanceTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceTracker.Infrastructure.Brokers.Storages.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired();
            builder.Property(c => c.Type).HasConversion<string>();

            builder.HasData(
                new Category { Id = 1, Name = "Food", Type = Domain.Enums.CategoryType.Food },
                new Category { Id = 2, Name = "Salary", Type = Domain.Enums.CategoryType.Salary},
                new Category { Id = 3, Name = "Transport", Type = Domain.Enums.CategoryType.Transportation},
                new Category { Id = 4, Name = "Rent", Type = Domain.Enums.CategoryType.Rent },
                new Category { Id = 5, Name = "Utilities", Type = Domain.Enums.CategoryType.Utilities},
                new Category { Id = 6, Name = "Entertainment", Type = Domain.Enums.CategoryType.Entertainment },
                new Category { Id = 7, Name = "Other", Type = Domain.Enums.CategoryType.Other },
                new Category { Id = 8, Name = "Shopping", Type = Domain.Enums.CategoryType.Shopping },
                new Category { Id = 9, Name = "Health", Type = Domain.Enums.CategoryType.Health },
                new Category { Id = 10, Name = "Investment", Type = Domain.Enums.CategoryType.Investment },
                new Category { Id = 11, Name = "Bills", Type = Domain.Enums.CategoryType.Bills },
                new Category { Id = 12, Name = "Bonus", Type = Domain.Enums.CategoryType.Bonus }
            );
        }
    }
}
