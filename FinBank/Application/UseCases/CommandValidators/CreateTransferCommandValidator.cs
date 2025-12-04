using Application.UseCases.Commands;
using Application.UseCases.Commands.TransferCommands;
using FluentValidation;

namespace Application.UseCases.CommandValidators;

public sealed class CreateTransferCommandValidator : AbstractValidator<CreateTransferCommand>
{
    public CreateTransferCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.Iban) .NotEmpty();

        RuleFor(x => x.ToIban)
            .NotEmpty()
            .Must((cmd, v) => !string.Equals(v, cmd.Iban, StringComparison.OrdinalIgnoreCase))
            .WithMessage("From/To accounts must differ.");

        RuleFor(x => x.Amount)
            .GreaterThan(0m)
            .Must(a => DecimalScale(a) <= 2).WithMessage("Amount supports max 2 decimals.");

        RuleFor(x => x.Currency)
            .NotEmpty().Length(3)
            .Must(IsUpperAscii).WithMessage("Currency must be ISO-4217 (uppercase).");

        RuleFor(x => x.IdempotencyKey)
            .NotEmpty().MaximumLength(100);
    }

    private static int DecimalScale(decimal value)
    {
        var bits = decimal.GetBits(value);
        return (bits[3] >> 16) & 0xFF;
    }

    private static bool IsUpperAscii(string ccy) => ccy.All(ch => ch is >= 'A' and <= 'Z');
}