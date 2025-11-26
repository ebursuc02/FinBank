using Application.DTOs;
using Domain;
using FluentResults;
using Mediator.Abstractions; 

namespace Application.UseCases.Commands.UserCommands;

public class RegisterUserCommand() : ICommand<Result<User>>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string? Country { get; init; }
    public DateTime? Birthday { get; init; }
    public  string? Address { get; init; }
}
