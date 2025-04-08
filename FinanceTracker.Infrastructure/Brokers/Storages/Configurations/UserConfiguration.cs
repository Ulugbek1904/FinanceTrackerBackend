using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Brokers.Storages.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
            builder.Property(u => u.Password).IsRequired();
            builder.HasMany(u => u.Accounts)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            var superAdminId = Guid.Parse("11111111-1111-1111-1111-111111111111"); 
            var createdAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc); 

            builder.HasData(new User
            {
                Id = superAdminId, 
                Email = "julugbek023@gmail.com",
                Password = "Qwerty1904",
                FirstName = "Super",
                LastName = "Admin",
                Role = Role.SuperAdmin,
                CreatedAt = createdAt, 
                IsActive = true
            });

        }
    }

}
