using FluentValidation;
using Library.Application.DTOs;

namespace Library.Application.Validators
{
    public class AuthorDTOValidator : AbstractValidator<AuthorDTO>
    {
        public AuthorDTOValidator()
        {
            RuleFor(authorDTO => authorDTO.Id)
                .NotEmpty().WithMessage("Author ID is required.");

            RuleFor(authorDTO => authorDTO.Name)
                .NotEmpty().WithMessage("Author name is required.")
                .MaximumLength(50).WithMessage("Author name cannot exceed 50 characters.");

            RuleFor(authorDTO => authorDTO.Birthday)
                .NotEmpty().WithMessage("Author birthday is required.")
                .LessThan(DateTime.Today).WithMessage("Author birthday cannot be in the future.");

            RuleFor(authorDTO => authorDTO.Country)
                .NotEmpty().WithMessage("Country is required.")
                .MaximumLength(50).WithMessage("Country name cannot exceed 50 characters.");
        }
    }
}