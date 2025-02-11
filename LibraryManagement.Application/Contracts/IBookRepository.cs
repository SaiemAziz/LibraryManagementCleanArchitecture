using System;
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Application.Contracts;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(Guid id);
    Task<List<Book>> GetAllAsync();
    Task AddAsync(Book book);
    Task UpdateAsync(Book book);
    Task DeleteAsync(Book book);
    // Add other book-specific repository methods as needed (e.g., FindByISBNAsync)
}