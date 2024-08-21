using FluentValidation;
using Library.Domain.DTOs;
using Library.Domain.Validators.ValidatorsHelpers;

namespace Library.Domain.Validators
{
    public class BookDTOValidator : AbstractValidator<BookDTO>
    {
        public BookDTOValidator()
        {
            RuleFor(bookDTO => bookDTO.Id)
                .NotEmpty().WithMessage("Book ID is required.");

            RuleFor(bookDTO => bookDTO.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name cannot be longer than 50 characters.");


            RuleFor(bookDTO => bookDTO.Description)
                .MaximumLength(500).WithMessage("Description cannot be longer than 500 characters.");

            RuleFor(bookDTO => bookDTO.Author)
                .SetValidator(new AuthorValidator());

            RuleFor(bookDTO => bookDTO.ImageFile)
                .NotNull().WithMessage("Image file is required.")
                .Must(BookValidHelper.BeAValidImage).WithMessage("Invalid image file format.");
        }
    }
}