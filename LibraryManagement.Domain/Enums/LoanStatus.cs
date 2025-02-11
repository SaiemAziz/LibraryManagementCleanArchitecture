namespace LibraryManagement.Domain.Enums;

public enum LoanStatus
{
    Active,      // Loan is currently active
    Overdue,     // Loan is active but past due date
    Returned,    // All items in the loan have been returned
    Closed       // Loan is fully closed and completed

}
