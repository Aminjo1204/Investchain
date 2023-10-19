using Bogus;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace InvestchainApp.Application.Infrastructure
{
    public class InvestchainContext : DbContext
    {
        public InvestchainContext(DbContextOptions opt) : base(opt)
        {
        }

        public DbSet<Currency> Currencies => Set<Currency>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Handin>().HasIndex("TaskId", "StudentId").IsUnique();
            //modelBuilder.Entity<Task>().HasIndex(nameof(Task.Title), "TeamId").IsUnique();
            //// Es sollen DateTimeKind UTC beim zurücklesen gesetzt werden.
            //modelBuilder.Entity<Task>()
            //    .Property(t => t.ExpirationDate)
            //    .HasConversion(
            //        v => v,   // 1:1 in die DB schreiben
            //        v => new DateTime(v.Ticks, DateTimeKind.Utc));  // auslesen als UTC

            // Generic config for all entities
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // ON DELETE RESTRICT instead of ON DELETE CASCADE
                foreach (var key in entityType.GetForeignKeys())
                    key.DeleteBehavior = DeleteBehavior.Restrict;

                foreach (var prop in entityType.GetDeclaredProperties())
                {
                    // Define Guid as alternate key. The database can create a guid fou you.
                    if (prop.Name == "Guid")
                    {
                        modelBuilder.Entity(entityType.ClrType).HasAlternateKey("Guid");
                        prop.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd;
                    }
                    // Default MaxLength of string Properties is 255.
                    if (prop.ClrType == typeof(string) && prop.GetMaxLength() is null) prop.SetMaxLength(255);
                    // Seconds with 3 fractional digits.
                    if (prop.ClrType == typeof(DateTime)) prop.SetPrecision(3);
                    if (prop.ClrType == typeof(DateTime?)) prop.SetPrecision(3);
                }
            }
        }

        public async Task Seed()
        {
            Randomizer.Seed = new Random(1804);

            Currency[] currencies = new[]
            {
                new Currency(name: "BTC"),
                new Currency(name: "ETH"),
                new Currency(name: "USDT"),
                new Currency(name: "BNB"),
            };

            await Currencies.AddRangeAsync(currencies);
            await SaveChangesAsync();
        }
    }
}
