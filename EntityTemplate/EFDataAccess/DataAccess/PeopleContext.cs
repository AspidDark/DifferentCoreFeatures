using EFDataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace EFDataAccess.DataAccess
{
    //add-migration
    //update-database
    public class PeopleContext : DbContext
    {
        public PeopleContext(DbContextOptions options) : base(options)
        {
            // Database.EnsureCreated();   // создаем базу данных !!без возможности миграций!! только прототипирование при первом обращении
           // Database.Migrate(); // создаем базу данных !!с возможностью миграций!!
        }

        public DbSet<Person> People { get; set; }

        public DbSet<Address> Addresses { get; set; }

        public DbSet<Email> EmailAddresses { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Product> Products { get; set; }

       // public DbSet<CustomerProduct> CustomersProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerProduct>()
            .HasKey(bc => new { bc.ProductId, bc.CustomerId });
            modelBuilder.Entity<CustomerProduct>()
                .HasOne(bc => bc.Customer)
                .WithMany(b => b.CustomerProducts)
                .HasForeignKey(bc => bc.CustomerId);
            modelBuilder.Entity<CustomerProduct>()
                .HasOne(bc => bc.Product)
                .WithMany(c => c.CustomerProducts)
                .HasForeignKey(bc => bc.ProductId);
        }

    }
}
