using FluentValidation;

namespace AqlliAgronom.Application.Features.Auth.Commands.LoginFarmer;

public class LoginFarmerCommandValidator : AbstractValidator<LoginFarmerCommand>
{
    public LoginFarmerCommandValidator()
    {
        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[1-9]\d{9,14}$").WithMessage("Invalid phone number format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
