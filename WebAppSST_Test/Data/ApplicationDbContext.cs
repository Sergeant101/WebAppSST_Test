using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.IO;
using WebAppSST_Test.Models;

namespace WebAppSST_Test.Data
{
    public class ApplicationDbContext : DbContext
    {

        // string connections transmitted in the program
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }

        // using fluent API for configuration DB
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // primary key in table User -> ID
            modelBuilder.Entity<User>()
                .HasKey(u => u.ID);

            // ID is not null
            modelBuilder.Entity<User>()
                .Property(u => u.ID)
                .IsRequired();

            // Name is not null and max string[15]
            modelBuilder.Entity<User>()
                .Property(n => n.Name)
                .IsRequired()
                .HasMaxLength(15);

            // SurName is not null and max string[30]
            modelBuilder.Entity<User>()
                .Property(s => s.SurName)
                .IsRequired()
                .HasMaxLength(30);

        }
    }
}
