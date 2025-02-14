using System;
using LibraryManagement.Application.Contracts;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Persistence.Repositories;

public class BookRepository(LibraryDbContext dbContext) : IBookRepository // Implement IBookRepository interface
{
    private readonly LibraryDbContext _dbContext = dbContext;

    public async Task<Book?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Books
            .Include(b => b.Author) // Eagerly load Author navigation property
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<List<Book>> GetAllAsync()
    {
        return await _dbContext.Books
            .Include(b => b.Author) // Eagerly load Author
            .ToListAsync();
    }

    public async Task AddAsync(Book book)
    {
        await _dbContext.Books.AddAsync(book);
        await _dbContext.SaveChangesAsync(); // Save changes to database
    }

    public async Task UpdateAsync(Book book)
    {
        _dbContext.Books.Update(book);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Book book)
    {
        _dbContext.Books.Remove(book);
        await _dbContext.SaveChangesAsync();
    }

    // ... Implement other methods from IBookRepository (e.g., FindByISBNAsync, GetAvailableBooksByGenreAsync using Specifications)
}