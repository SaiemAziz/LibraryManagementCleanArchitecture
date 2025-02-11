using System;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.Models;

public class BookDetailsDto
{
    public Guid Id { get; set; }
    public string? ISBN { get; set; }
    public string? Title { get; set; }
    public string? AuthorName { get; set; } // Flattened Author Name for presentation
    public int PublicationYear { get; set; }
    public Genre Genre { get; set; }
    public bool IsAvailable { get; set; }
}