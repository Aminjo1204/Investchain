using Bogus;
using InvestChainApp.Application.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestChainApp.Application.Infastructure
{
    public class InvestChainContext : DbContext
    {
        public DbSet<User> Users => Set<User>();

        public InvestChainContext(DbContextOptions opt) : base(opt)
        {
        }

        //OnModelCreate
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }

        public void Seed()
        {
            Randomizer.Seed = new Random(1024);
            var faker = new Faker("de");

            var users = new Faker<User>("de").CustomInstantiator(f =>
            {
                return new User(
                    mail: f.Name.FirstName() + "@gmail.com",
                    username: f.Name.FirstName(),
                    initialPassword: "1111")
                { Guid = f.Random.Guid() };
            })
            .Generate(10)
            .ToList();
            Users.AddRange(users);
            SaveChanges();
        }

        private void Initialize()
        {

        }

        public void CreateDatabase(bool isDevelopment)
        {
            if (isDevelopment) { Database.EnsureDeleted(); }
            // EnsureCreated only creates the model if the database does not exist or it has no
            // tables. Returns true if the schema was created.  Returns false if there are
            // existing tables in the database. This avoids initializing multiple times.
            if (Database.EnsureCreated()) { Initialize(); }
            if (isDevelopment) Seed();
        }








    }
}
