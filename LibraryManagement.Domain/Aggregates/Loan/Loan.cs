using System;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.ValueObjects;

namespace LibraryManagement.Domain.Aggregates.Loan;

public class Loan(Guid memberId, LoanDate loanDate)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid MemberId { get; private set; } = memberId;
    public Member Member { get; private set; } = default!;
    private readonly List<LoanItem> _loanItems = [];
    public IReadOnlyCollection<LoanItem> LoanItems => _loanItems.AsReadOnly();
    public LoanDate LoanDate { get; private set; } = loanDate;
    public LoanDate? ReturnDate { get; private set; }
    public LoanStatus Status { get; private set; } = LoanStatus.Active;

    // Domain Behavior - Example: Add a book to the loan
    public void AddLoanItem(Book book, LoanDate dueDate)
    {
        if (Status != LoanStatus.Active)
        {
            throw new InvalidOperationException("Cannot add items to a closed loan.");
        }
        if (!book.IsAvailable)
        {
            throw new InvalidOperationException($"Book '{book.Title}' is not available for loan.");
        }

        _loanItems.Add(new LoanItem(book.Id, dueDate));
        book.BorrowBook(MemberId, /* You would need to resolve IPublisher here properly in a real app, e.g., via Domain Event Dispatcher pattern */ null!); // Example: Passing null publisher for now - see note below
    }

    // Domain Behavior - Example: Return a loan item (book)
    public void ReturnLoanItem(Guid bookId, LoanDate actualReturnDate)
    {
        var loanItem = _loanItems.FirstOrDefault(item => item.BookId == bookId);
        if (loanItem == null)
        {
            throw new InvalidOperationException($"Book with ID '{bookId}' is not part of this loan.");
        }

        if (Status != LoanStatus.Active)
        {
            throw new InvalidOperationException("Cannot return items for a closed loan.");
        }

        loanItem.MarkAsReturned(actualReturnDate);
        // In a real app, you'd fetch the Book entity and call book.ReturnBook();
        // For this example, we'll just set loan status to closed when all items are returned
        if (_loanItems.All(item => item.IsReturned))
        {
            CloseLoan();
        }
    }

    // Domain Behavior - Example: Close the loan
    public void CloseLoan()
    {
        if (Status == LoanStatus.Closed)
        {
            throw new InvalidOperationException("Loan is already closed.");
        }
        Status = LoanStatus.Closed;
        ReturnDate = new LoanDate(DateTime.Now);
    }

    // Domain Behavior - Example: Mark Loan as Overdue (Illustrative - in real app, this would likely be triggered by a background process or application service)
    public void MarkAsOverdue()
    {
        if (Status == LoanStatus.Active)
        {
            Status = LoanStatus.Overdue;
            // In a real application, you might raise a Domain Event here, e.g., LoanOverdueEvent
        }
    }
}