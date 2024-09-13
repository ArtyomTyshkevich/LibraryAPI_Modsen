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

            RuleFor(bookDTO => bookDTO.ImageFile)
                .NotNull().WithMessage("Image file is required.")
                .Must(BookValidHelper.BeAValidImage).WithMessage("Invalid image file format.");
        }
    }
}