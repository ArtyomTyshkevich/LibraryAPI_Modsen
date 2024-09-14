using FluentValidation;
using Library.Application.DTOs;
using Library.Application.Validators.ValidatorsHelpers;


namespace Library.Application.Validators
{
    public class BookDTOValidator : AbstractValidator<BookDTO>
    {
        public BookDTOValidator()
        {
            RuleFor(bookDTO => bookDTO.Id)
                .NotEmpty().WithMessage("Book ID is required.");

            RuleFor(book => book.ISBN)
                .NotEmpty().WithMessage("ISBN is required.")
                .Must(BookValidHelper.IsValidISBN!).WithMessage("Invalid ISBN format.");

            RuleFor(bookDTO => bookDTO.Name)
                .NotEmpty().WithMessage("Book name is required.")
                .MaximumLength(100).WithMessage("Book name cannot exceed 100 characters.");

            RuleFor(bookDTO => bookDTO.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(bookDTO => bookDTO.ImageFile)
                .NotNull().WithMessage("Image file is required.")
                .Must(BookValidHelper.BeAValidImage).WithMessage("Invalid image file format.");
        }
    }
}