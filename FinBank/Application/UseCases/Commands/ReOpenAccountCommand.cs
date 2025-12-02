using Application.Interfaces.Utils;
using Mediator.Abstractions;
using FluentResults;

namespace Application.UseCases.Commands
{
    public class ReOpenAccountCommand : ICommand<Result>, IAuthorizable
    {
        public Guid CustomerId { get; init; }
        public string Iban { get; init; } = string.Empty;
    }
}
