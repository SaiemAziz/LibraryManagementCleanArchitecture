using System;
using System.Linq.Expressions;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.Specifications;

public class AvailableBooksOfGenreSpecification
{
    public Genre Genre { get; }

    public AvailableBooksOfGenreSpecification(Genre genre)
    {
        Genre = genre;
    }

    public Expression<Func<Book, bool>> Criteria()
    {
        return book => book.IsAvailable && book.Genre == Genre;
    }
}
