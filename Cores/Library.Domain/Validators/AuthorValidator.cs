using FluentValidation;
using Library.Domain.Entities;


namespace Library.Domain.Validators
{
    public class AuthorValidator : AbstractValidator<Author>
    {
        public AuthorValidator()
        {
            RuleFor(author => author.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(20).WithMessage("Name cannot be longer than 20 characters.");
            
            RuleFor(author => author.Birthday)
                .LessThanOrEqualTo(DateTime.Today)
                .WithMessage("Birthday must be in the past.");

            RuleFor(author => author.Country)
                .NotEmpty().WithMessage("Country is required.")
                .MaximumLength(30).WithMessage("Country cannot be longer than 30 characters.");
        }
    }
}
