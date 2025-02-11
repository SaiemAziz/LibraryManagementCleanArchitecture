using System;
using FluentValidation;
using LibraryManagement.Application.Requests;

namespace LibraryManagement.Application.Validators;

public class BorrowBookCommandValidator : AbstractValidator<BorrowBookCommand> // Inherit from AbstractValidator
{
    public BorrowBookCommandValidator()
    {
        RuleFor(command => command.BookId) // Validation rule for BookId
            .NotEmpty().WithMessage("Book ID is required.")
            .NotEqual(Guid.Empty).WithMessage("Book ID must be a valid GUID.");

        RuleFor(command => command.MemberId) // Validation rule for MemberId
            .NotEmpty().WithMessage("Member ID is required.")
            .NotEqual(Guid.Empty).WithMessage("Member ID must be a valid GUID.");
    }
}
