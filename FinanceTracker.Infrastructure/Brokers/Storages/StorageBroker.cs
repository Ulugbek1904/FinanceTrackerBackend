﻿using FinanceTracker.Domain.Models;
using FinanceTracker.Infrastructure.Brokers.Storages.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FinanceTracker.Infrastructure.Brokers.Storages
{
    public class StorageBroker : DbContext, IStorageBroker
    {
        private readonly IConfiguration configuration;
        public StorageBroker( DbContextOptions<StorageBroker> options,
            IConfiguration configuration) : base(options)
        {
            this.configuration = configuration;
            this.Database.MigrateAsync();
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Transaction> Transactions => Set<Transaction>();

        public async ValueTask<T> InsertAsync<T>(T entity) where T : class
        {
            await this.Set<T>().AddAsync(entity);
            await this.SaveChangesAsync();

            return entity;
        }

        public IQueryable<T> SelectAll<T>() where T : class
            => Set<T>().AsQueryable();

        public async ValueTask<T?> SelectByKeyAsync<T>(params object[] keyValues) where T : class
            => await this.Set<T>().FindAsync(keyValues);

        public async ValueTask<T> SelectByIdAsync<T>(Guid id) where T : class
            => await this.Set<T>().FindAsync(id);

        public async ValueTask<T> DeleteAsync<T>(T entity) where T : class
        {
            this.Set<T>().Remove(entity);
            await this.SaveChangesAsync();

            return entity;
        }

        public async ValueTask<T> UpdateAsync<T>(T entity) where T : class
        {
            this.Set<T>().Update(entity);
            await this.SaveChangesAsync();

            return entity;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = this.configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
