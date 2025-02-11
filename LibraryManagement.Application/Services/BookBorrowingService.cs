using System;
using LibraryManagement.Application.Contracts;

namespace LibraryManagement.Application.Services;

public class BookBorrowingService
{
    private readonly IBookRepository _bookRepository;
    private readonly IEmailService _emailService; // Dependency on Email Service Abstraction
    // In a real app, you'd likely have a IMemberRepository as well

    public BookBorrowingService(IBookRepository bookRepository, IEmailService emailService) // Dependency Injection
    {
        _bookRepository = bookRepository;
        _emailService = emailService;
    }

    public async Task BorrowBookAsync(Guid bookId, Guid memberId)
    {
        var book = await _bookRepository.GetByIdAsync(bookId);
        if (book == null)
        {
            throw new InvalidOperationException($"Book with ID '{bookId}' not found."); // Or a custom exception
        }

        // Domain Logic: Borrow the book (call Domain Entity method)
        book.BorrowBook(memberId, /* In real app, you'd resolve IPublisher properly via DI in Application Layer and pass it here */ null!); // Example: Passing null publisher for now - see note below

        // Persist changes using Repository
        await _bookRepository.UpdateAsync(book);

        // Infrastructure Service Usage: Send confirmation email (using abstraction)
        await _emailService.SendEmailAsync(
            "member@example.com", // In real app, get member email from Member entity
            "Book Borrowed",
            $"You have borrowed the book: {book.Title}"
        );

        // Transaction handling would typically be managed at a higher level (e.g., in a Command Handler or Unit of Work)
    }
}