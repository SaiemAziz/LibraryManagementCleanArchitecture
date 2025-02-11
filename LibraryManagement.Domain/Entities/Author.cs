using System;

namespace LibraryManagement.Domain.Entities;

public class Author(string firstName, string lastName, string? biography = null)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string FirstName { get; private set; } = firstName;
    public string LastName { get; private set; } = lastName;
    public string? Biography { get; private set; } = biography;

    // Method to update biography (example of domain behavior)
    public void UpdateBiography(string biography)
    {
        Biography = biography;
    }
}
