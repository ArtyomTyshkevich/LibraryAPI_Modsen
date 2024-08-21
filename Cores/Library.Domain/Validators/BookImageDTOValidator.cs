using FluentValidation;
using Library.Domain.DTOs;
using Library.Domain.Entities;
using Library.Domain.Validators.ValidatorsHelpers;
using System;

namespace Library.Domain.Validators
{
    public class BookImageDTOValidator : AbstractValidator<BookImageDTO>
    {
        public BookImageDTOValidator()
        {
            RuleFor(bookImageDTO => bookImageDTO.Id)
                .NotEmpty().WithMessage("Book ID is required.");

            RuleFor(bookImageDTO => bookImageDTO.ImageFile)
                .NotNull().WithMessage("Image file is required.")
                .Must(BookValidHelper.BeAValidImage).WithMessage("Invalid image file format.");
        }
    }
}