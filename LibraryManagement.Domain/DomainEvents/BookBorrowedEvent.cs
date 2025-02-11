using System;
using LibraryManagement.Domain.Entities;
using MediatR;

namespace LibraryManagement.Domain.DomainEvents;

public class BookBorrowedEvent : INotification // Implement INotification
{
    public Book Book { get; }
    public Guid MemberId { get; }
    public DateTime BorrowDate { get; }

    public BookBorrowedEvent(Book book, Guid memberId, DateTime borrowDate)
    {
        Book = book;
        MemberId = memberId;
        BorrowDate = borrowDate;
    }
}
