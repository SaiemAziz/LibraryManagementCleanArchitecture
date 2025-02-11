using System;
using MediatR;

namespace LibraryManagement.Application.Requests;

public class BorrowBookCommand : IRequest // Using MediatR's IRequest (no return value for command)
{
    public Guid BookId { get; }
    public Guid MemberId { get; }

    public BorrowBookCommand(Guid bookId, Guid memberId)
    {
        BookId = bookId;
        MemberId = memberId;
    }
}