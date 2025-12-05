using Application.UseCases.Commands;
using FluentValidation;

namespace Application.UseCases.CommandValidators;

public class GetRiskStatusCommandValidators : AbstractValidator<GetRiskStatusCommand>
{
    public GetRiskStatusCommandValidators()
    {
        RuleFor(x => x.UserCnp).NotEmpty().WithMessage("UserCnp is required");
        RuleFor(x => x.UserCnp).Length(13).WithMessage("Cnp must have 13 digits");
        RuleFor(x => x.UserCnp).Matches("^\\d{13}$").WithMessage("Cnp must contain only digits");
    }
}