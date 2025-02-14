using System;
using LibraryManagement.Domain.Aggregates.Loan;
using LibraryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Persistence.DataContext;

public class LibraryDbContext(DbContextOptions<LibraryDbContext> options) : DbContext(options)
{
    public DbSet<Book> Books { get; set; } // DbSet for Book entity
    public DbSet<Author> Authors { get; set; } // DbSet for Author entity
    public DbSet<Member> Members { get; set; } // DbSet for Member entity
    public DbSet<Loan> Loans { get; set; }     // DbSet for Loan aggregate root

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships, indexes, constraints, etc. here using modelBuilder API
        // Example: Configure Book-Author relationship (though EF Core might infer this by convention)
        modelBuilder.Entity<Book>()
            .HasOne(b => b.Author)
            .WithMany() // Assuming Author doesn't have a navigation property back to Book for simplicity
            .HasForeignKey(b => b.AuthorId);

        // Example: Configure Loan-LoanItem relationship (if needed - EF Core might infer)
        modelBuilder.Entity<Loan>()
            .OwnsMany(l => l.LoanItems); // Loan owns LoanItems (Value Objects)

        // Example: Configure value object as owned entity (if LoanDate was an owned entity, not Value Object - for demonstration)
        // modelBuilder.Entity<Loan>().OwnsOne(l => l.LoanDate); // If LoanDate was an Owned Entity
    }
}