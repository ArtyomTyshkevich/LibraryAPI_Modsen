using FluentValidation;
using Library.Domain.Entities;

namespace Library.Domain.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(user => user.FirstName)
                .NotEmpty().WithMessage("First Name is required.")
                .MaximumLength(50).WithMessage("First Name cannot be longer than 50 characters.");

            RuleFor(user => user.LastName)
                .NotEmpty().WithMessage("Last Name is required.")
                .MaximumLength(50).WithMessage("Last Name cannot be longer than 50 characters.");

            RuleFor(user => user.MiddleName)
                .MaximumLength(50).WithMessage("Middle Name cannot be longer than 50 characters.")
                .When(user => !string.IsNullOrEmpty(user.MiddleName));

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

        }
    }
}