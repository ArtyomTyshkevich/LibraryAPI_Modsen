using FluentValidation;
using Library.Domain.Entities;
using Library.Domain.Validators.ValidatorsHelpers;
using System;

namespace Library.Domain.Validators
{
    public class BookValidator : AbstractValidator<Book>
    {
        public BookValidator()
        {
            RuleFor(book => book.ISBN)
                .NotEmpty().WithMessage("ISBN is required.")
                .Must(BookValidHelper.IsValidISBN).WithMessage("Invalid ISBN format.");

            RuleFor(book => book.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name cannot be longer than 50 characters.");


            RuleFor(book => book.Description)
                .MaximumLength(500).WithMessage("Description cannot be longer than 500 characters.");

            RuleFor(book => book.Author)
                .SetValidator(new AuthorValidator());

            RuleFor(book => book.StartRentDateTime)
                .LessThanOrEqualTo(book => book.EndRentDateTime)
                .WithMessage("Start rent date must be before or the same as the end rent date.");

            RuleFor(book => book.EndRentDateTime)
                .GreaterThanOrEqualTo(DateTime.Now)
                .WithMessage("End rent date cannot be in the past.");

            RuleFor(book => book.ImageFileName)
                .Must(BookValidHelper.BeAValidFileName).WithMessage("Invalid file name format.")
                .When(book => !string.IsNullOrEmpty(book.ImageFileName));
        }
    }
}