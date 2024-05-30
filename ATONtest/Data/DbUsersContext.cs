using ATONtest.Models;
using Microsoft.EntityFrameworkCore;

namespace ATONtest.Data
{
    public class DbUsersContext : DbContext
    {
        public DbUsersContext(DbContextOptions<DbUsersContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {      
                entity.Property(e => e.Id)
                .ValueGeneratedOnAdd(); //Guid autoicrement

                entity.Property(e => e.Gender)
                .IsRequired();

                entity.Property(e => e.Name)
                .IsRequired();

                entity.Property(e => e.Birthday)
                .HasColumnType("date");

                entity.Property(e => e.Login)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsFixedLength(false) 
                    .HasAnnotation("RegularExpression", "^[a-zA-Z0-9]*$"); // Regex для латинских букв и цифр

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasAnnotation("RegularExpression", "^[a-zA-Z0-9]*$"); // Regex для латинских букв и цифр

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasAnnotation("RegularExpression", "^[a-zA-Zа-яА-Я]*$"); // Regex для латинских и русских букв
               
                entity.HasIndex(e => e.Login)
                .IsUnique(); // index unique

                entity.Property(e => e.CreatedOn)
                .HasColumnType("date");

                entity.Property(e => e.ModifiedOn)
                .HasColumnType("date");

                entity.Property(e => e.RevokedOn)
                .HasColumnType("date");

            });
        }
    }
}


