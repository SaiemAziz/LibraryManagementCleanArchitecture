using System;
using LibraryManagement.Application.Models;
using MediatR;

namespace LibraryManagement.Application.Requests;

public class GetBookDetailsQuery : IRequest<BookDetailsDto> // IRequest<TResponse> for queries with return value
{
    public Guid BookId { get; }

    public GetBookDetailsQuery(Guid bookId)
    {
        BookId = bookId;
    }
}