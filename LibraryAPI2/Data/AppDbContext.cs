using LibraryAPI2.Controllers;
using LibraryAPI2.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LibraryAPI2.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Location>? Locations { get; set; }
        public DbSet<Language>? Languages { get; set; }
        public DbSet<Category>? Categories { get; set; }    
        public DbSet<SubCategory>? SubCategories { get; set; }
        public DbSet<Publisher>? Publishers { get; set; }
        public DbSet<Author>? Authors { get; set; }
        public DbSet<Book>? Books { get; set; }
        public DbSet<Member>? Members { get; set; }
        public DbSet<Employee>? Employees { get; set; }
        public DbSet<RentedBook> RentedBooks { get; set; } = default!;
        public DbSet<AuthorBook>? AuthorBooks { get; set; }
        public DbSet<BookRating>? BookRatings { get; set; }
        public DbSet<BookStock>? BookStocks { get; set; }
        public DbSet<BookCopy>? BookCopies { get; set; }
        public DbSet<Penalty>? Penalties { get; set; }
        public DbSet<Donator>? Donators { get; set; }    
        public DbSet<DonatedBook>? DonatedBooks { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AuthorBook>().HasKey(a => new { a.AuthorsId, a.BooksId });
            modelBuilder.Entity<AuthorBook>()
                .HasOne(a => a.Book)
                .WithMany(a => a.AuthorBook)
                .HasForeignKey(a => a.BooksId);
            modelBuilder.Entity<AuthorBook>()
                .HasOne(a => a.Author)
                .WithMany(a => a.AuthorBooks)
                .HasForeignKey(a => a.AuthorsId);
            modelBuilder.Entity<Category>()
                .HasMany(c => c.SubCategories)
                .WithOne(c => c.Category)
                .HasForeignKey(c => c.CategoryId);

            modelBuilder.Entity<AppUser>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<AppUser>()
                .HasIndex(x => x.IdNumber)
                .IsUnique();

            modelBuilder.Entity<AppUser>()
                .HasIndex(x => x.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<AppUser>()
                .HasIndex(x => x.UserName)
                .IsUnique();

            modelBuilder.Entity<Donator>()
                .HasIndex(x => x.Email)
                .IsUnique();
                

            //
            modelBuilder.Entity<Book>()
                .HasOne(x => x.SubCategory)
                .WithMany(x => x.Books)
                .HasForeignKey(x => x.SubCategoryId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<SubCategory>()
                .HasOne(x => x.Category)
                .WithMany(x => x.SubCategories)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
            //
            modelBuilder.Entity<RentedBook>().HasKey(rb => new { rb.MemberId, rb.BookId, rb.CopyId });

            modelBuilder.Entity<RentedBook>()
                .HasOne(x => x.Book)
                .WithMany()
                .HasForeignKey(x => x.BookId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RentedBook>()
                .HasOne(x => x.AppUser)
                .WithMany()
                .HasForeignKey(x => x.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RentedBook>()
                .HasOne(x => x.BookCopy)
                .WithMany()
                .HasForeignKey(x => x.CopyId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<BookRating>()
                .HasOne(x => x.Book)
                .WithMany()
                .HasForeignKey(x => x.BookId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<BookStock>()
                .HasOne(x => x.Book)
                .WithMany()
                .HasForeignKey(x => x.BookId);

           

            modelBuilder.Entity<BookCopy>()
                .HasOne(x => x.Donator)
                .WithMany(x=> x.BookCopies)
                .HasForeignKey(x => x.DonatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DonatedBook>()
                .HasOne(db => db.Book)
                .WithMany()
                .HasForeignKey(db => db.BookId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DonatedBook>()
                .HasOne(db => db.Donator)
                .WithMany()
                .HasForeignKey(db => db.DonatorId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DonatedBook>()
                .HasOne(db => db.AppUser)
                .WithMany()
                .HasForeignKey(db => db.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Donator>()
                .HasIndex(x => x.Email)
                .IsUnique();





        }



    }
}
