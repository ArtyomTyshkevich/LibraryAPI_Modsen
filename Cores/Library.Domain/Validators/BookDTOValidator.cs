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

            RuleFor(bookDTO => bookDTO.ImageFile)
                .NotNull().WithMessage("Image file is required.")
                .Must(BookValidHelper.BeAValidImage).WithMessage("Invalid image file format.");
        }
    }
}