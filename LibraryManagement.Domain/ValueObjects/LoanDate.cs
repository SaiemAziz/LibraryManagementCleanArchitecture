using System;

namespace LibraryManagement.Domain.ValueObjects;

public class LoanDate
{
    public DateTime Date { get; private set; }

    public LoanDate(DateTime date)
    {
        Date = date;
        if (Date < DateTime.Now.Date) // Validation: Loan date cannot be in the past
        {
            throw new ArgumentException("Loan date cannot be in the past.");
        }
        if (Date > DateTime.Now.Date.AddYears(1)) // Validation: Loan date cannot be more than one year in the future
        {
            throw new ArgumentException("Loan date cannot be more than one year in the future.");
        }
    }

    // Override Equals and GetHashCode for value-based equality
    public override bool Equals(object? obj)
    {
        return obj is LoanDate other && Date == other.Date;
    }

    public override int GetHashCode()
    {
        return Date.GetHashCode();
    }

    public override string ToString()
    {
        return Date.ToString("yyyy-MM-dd"); // Consistent date formatting
    }

    // Implicit conversion to DateTime (optional, use with caution)
    public static implicit operator DateTime(LoanDate loanDate) => loanDate.Date;
    public static implicit operator LoanDate(DateTime dateTime) => new LoanDate(dateTime);
}
