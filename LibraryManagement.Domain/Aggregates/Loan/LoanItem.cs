using System;
using LibraryManagement.Domain.ValueObjects;

namespace LibraryManagement.Domain.Aggregates.Loan;

public class LoanItem(Guid bookId, LoanDate dueDate)
{
    public Guid BookId { get; private set; } = bookId;
    public LoanDate DueDate { get; private set; } = dueDate;
    public LoanDate? ActualReturnDate { get; private set; } // Nullable Value Object
    public bool IsReturned { get; private set; } = false;

    public void MarkAsReturned(LoanDate actualReturnDate)
    {
        if (IsReturned)
        {
            throw new InvalidOperationException("Loan item is already marked as returned.");
        }
        IsReturned = true;
        ActualReturnDate = actualReturnDate;
    }
}
