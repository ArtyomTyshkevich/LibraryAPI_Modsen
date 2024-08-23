using FluentValidation;
using Library.Domain.Entities;

public class MassageValidator : AbstractValidator<Massage>
{
    public MassageValidator()
    {
        RuleFor(m => m.Desription)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(500)
            .WithMessage("Description must be less than 500 characters.");

        RuleFor(m => m.DepartureTime)
            .LessThanOrEqualTo(DateTime.Now)
            .WithMessage("Departure time must be in the past or present.");
    }
}