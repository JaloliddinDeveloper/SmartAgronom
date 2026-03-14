using FluentValidation;

namespace AqlliAgronom.Application.Features.Auth.Commands.RegisterFarmer;

public class RegisterFarmerCommandValidator : AbstractValidator<RegisterFarmerCommand>
{
    public RegisterFarmerCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(200).WithMessage("Full name must not exceed 200 characters.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[1-9]\d{9,14}$").WithMessage("Invalid phone number format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => x.Email is not null)
            .WithMessage("Invalid email address format.");
    }
}
