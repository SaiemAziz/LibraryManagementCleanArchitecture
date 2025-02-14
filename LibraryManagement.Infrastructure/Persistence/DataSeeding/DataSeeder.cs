using System;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagement.Infrastructure.Persistence.DataSeeding;

public static class DataSeeder
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();

        await dbContext.Database.MigrateAsync(); // Ensure database is created and migrations are applied

        if (!await dbContext.Authors.AnyAsync())
        {
            // Seed Authors
            var authors = new List<Author>
            {
                new("Jane", "Austen"),
                new("Agatha", "Christie"),
                new("J.R.R.", "Tolkien")
                // ... add more authors
            };
            await dbContext.Authors.AddRangeAsync(authors);
            await dbContext.SaveChangesAsync();
        }

        if (!await dbContext.Books.AnyAsync())
        {
            // Seed Books (assuming Authors are already seeded)
            var authors = await dbContext.Authors.ToListAsync(); // Get authors from DB
            var books = new List<Book>
            {
                new("978-0141439518", "Pride and Prejudice", authors[0].Id, 1813, Genre.Romance) { Author = authors[0] }, // Romance
                new("978-0007119374", "And Then There Were None", authors[1].Id, 1939, Genre.Mystery) { Author = authors[1] }, // Mystery
                new("978-0618260269", "The Hobbit", authors[2].Id, 1937, Genre.Fantasy) { Author = authors[2] }  // Fantasy
                // ... add more books
            };
            await dbContext.Books.AddRangeAsync(books);
            await dbContext.SaveChangesAsync();
        }

        // ... Seed other data (Members, etc.) if needed
    }
}