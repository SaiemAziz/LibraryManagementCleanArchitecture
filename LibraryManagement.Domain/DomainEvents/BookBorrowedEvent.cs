using System;
using LibraryManagement.Domain.Entities;
using MediatR;

namespace LibraryManagement.Domain.DomainEvents;

public class BookBorrowedEvent(Book book, Guid memberId, DateTime borrowDate) : INotification // Implement INotification
{
    public Book Book { get; } = book;
    public Guid MemberId { get; } = memberId;
    public DateTime BorrowDate { get; } = borrowDate;
}
