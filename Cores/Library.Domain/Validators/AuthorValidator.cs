using FluentValidation;
using Library.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Validators
{
    internal class AuthorValidator : AbstractValidator<Author>
    {
        public AuthorValidator()
        {
            RuleFor(author => author.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(20).WithMessage("Name cannot be longer than 20 characters.");

            RuleFor(author => author.Bithday)
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("Birthday must be in the past.");

            RuleFor(author => author.Country)
                .NotEmpty().WithMessage("Country is required.")
                .MaximumLength(30).WithMessage("Country cannot be longer than 50 characters.");
        }
    }
}
