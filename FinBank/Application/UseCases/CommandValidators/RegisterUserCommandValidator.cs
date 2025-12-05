using Application.UseCases.Commands.UserCommands;
using Domain;
using FluentValidation;

namespace Application.UseCases.CommandValidators;

public class RegisterUserCommandValidator: AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$")
            .WithMessage("Password must be at least 8 characters long and include uppercase, lowercase letters, and digits.");
        
        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.")
            .Must(role => role == UserRole.Customer || role == UserRole.Banker)
            .WithMessage("Invalid or unsupported role. Allowed roles: Customer, Banker.");
            
    }
}