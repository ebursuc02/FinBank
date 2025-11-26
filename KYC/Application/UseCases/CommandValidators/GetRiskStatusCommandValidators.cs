using Application.UseCases.Commands;
using FluentValidation;

namespace Application.UseCases.CommandValidators;

public class GetRiskStatusCommandValidators : AbstractValidator<GetRiskStatusCommand>
{
    public GetRiskStatusCommandValidators()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");
    }
}