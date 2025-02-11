using System;
using LibraryManagement.Domain.DomainEvents;
using LibraryManagement.Domain.Enums;
using MediatR;

namespace LibraryManagement.Domain.Entities;

public class Book(string isbn, string title, Guid authorId, int publicationYear, Genre genre)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string ISBN { get; private set; } = isbn;
    public string Title { get; private set; } = title;
    public Guid AuthorId { get; private set; } = authorId;
    public Author Author { get; private set; } = default!; // Navigation property
    public int PublicationYear { get; private set; } = publicationYear;
    public Genre Genre { get; private set; } = genre;
    public bool IsAvailable { get; private set; } = true;

    // Domain Behavior - Example: Borrow a book
    public void BorrowBook(Guid memberId, IPublisher publisher) // Inject IPublisher to publish event
    {
        if (!IsAvailable)
        {
            throw new InvalidOperationException($"Book '{Title}' (ISBN: {ISBN}) is not available for borrowing.");
        }
        IsAvailable = false;

        // Publish Domain Event when book is borrowed
        publisher.Publish(new BookBorrowedEvent(this, memberId, DateTime.Now));
    }

    // Domain Behavior - Example: Return a book
    public void ReturnBook()
    {
        if (IsAvailable)
        {
            throw new InvalidOperationException($"Book '{Title}' (ISBN: {ISBN}) is already available.");
        }
        IsAvailable = true;
    }

    // Example: Update Book Details (Illustrative Domain Behavior)
    public void UpdateDetails(string title, Genre genre, int publicationYear)
    {
        Title = title;
        Genre = genre;
        PublicationYear = publicationYear;
        // In a real application, you might add more validation and logic here
    }
}