using System;

namespace LibraryManagement.Domain.Constants;

public static class DomainConstants
{
    public const int MaxLoanDays = 30; // Maximum days a book can be loaned
    public const string DefaultCurrency = "USD"; // Example of a string constant (though may not be directly used in Domain in this scenario, but good example)
    public const double OverdueFinePerDay = 0.50; // Example: Overdue fine per day in USD
    public const int MaxLoanItemsPerMember = 5; // Example: Maximum books a member can borrow at once
}
