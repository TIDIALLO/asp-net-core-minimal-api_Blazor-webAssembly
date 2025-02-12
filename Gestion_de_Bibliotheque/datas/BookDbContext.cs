using System.IO.Compression;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gestion_de_Bibliothéque.datas
{
    public class BookDbContext : IdentityDbContext
    {
        public BookDbContext(DbContextOptions<BookDbContext> options) : base(options){}
        public DbSet<Book>? Books { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuration des entités d'Identity pour résoudre l'erreur de clé primaire
            modelBuilder.Entity<IdentityUserLogin<string>>()
                .HasKey(u => new { u.LoginProvider, u.ProviderKey });

            modelBuilder.Entity<IdentityUserRole<string>>()
                .HasKey(u => new { u.UserId, u.RoleId });

            modelBuilder.Entity<IdentityUserClaim<string>>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<IdentityRoleClaim<string>>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<IdentityUserToken<string>>()
                .HasKey(u => new { u.UserId, u.LoginProvider, u.Name });
            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    Id = 1,
                    Title = "sous l'orage",
                    Author = "Mariama Ba",
                    PubDate = new DateOnly(2000, 10, 29)
                },
                new Book
                {
                    Id = 2,
                    Title = "sous l'orage",
                    Author = "Mariama Ba",
                    PubDate = new DateOnly(2000, 10, 29)
                }
                );
        }   
    }

    
}
