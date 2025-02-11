using System;
using LibraryManagement.Domain.Aggregates.Loan;
using LibraryManagement.Domain.Constants;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.ValueObjects;

namespace LibraryManagement.Domain.Entities;

public class Member(string memberNumber, string firstName, string lastName, string email, DateTime dateOfBirth)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string MemberNumber { get; private set; } = memberNumber;
    public string FirstName { get; private set; } = firstName;
    public string LastName { get; private set; } = lastName;
    public string Email { get; private set; } = email;
    public DateTime DateOfBirth { get; private set; } = dateOfBirth;
    public DateTime RegistrationDate { get; private set; } = DateTime.Now; // Set registration date to current date
    public bool IsActive { get; private set; } = true;
    private readonly List<Loan> _loans = []; // Collection of Loans (Aggregate Roots)

    public IReadOnlyCollection<Loan> Loans => _loans.AsReadOnly(); // Read-only access to loans

    // Domain Behavior - Example: Borrow a Loan (starting a new loan)
    public Loan BorrowLoan(LoanDate loanDate)
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("Inactive members cannot borrow books.");
        }
        if (_loans.Count(l => l.Status == LoanStatus.Active) >= DomainConstants.MaxLoanItemsPerMember) // Using constant
        {
            throw new InvalidOperationException($"Member cannot borrow more than {DomainConstants.MaxLoanItemsPerMember} books at a time.");
        }

        var loan = new Loan(Id, loanDate); // Create a new Loan Aggregate Root
        _loans.Add(loan);
        return loan;
    }

    // Domain Behavior - Example: Deactivate Membership
    public void DeactivateMembership()
    {
        IsActive = false;
        // Could add logic to handle outstanding loans, etc. in a real application
    }
}
