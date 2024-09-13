using FluentValidation;
using Library.Application.DTOs;

namespace Library.Application.Validators
{
    public class UserDTOValidator : AbstractValidator<UserDTO>
    {
        public UserDTOValidator()
        {
            RuleFor(user => user.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(20).WithMessage("First name must not exceed 20 characters.");

            RuleFor(user => user.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(20).WithMessage("Last name must not exceed 20 characters.");

            RuleFor(user => user.MiddleName)
                .MaximumLength(20).WithMessage("Middle name must not exceed 20 characters.");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");
        }
    }
}