using Application.UseCases.Commands;
using FluentValidation;

namespace Application.UseCases.CommandValidators;

public class DeleteAccountCommandValidator : AbstractValidator<DeleteAccountCommand>
{
    public DeleteAccountCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.AccountIban).NotEmpty();
    }
}

