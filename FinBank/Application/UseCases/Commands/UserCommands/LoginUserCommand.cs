using Application.DTOs;
using Domain;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.Commands.UserCommands;

public class LoginUserCommand(string email, string password):ICommand<Result<UserDto>>
{
    public string Email => email;
    public string Password => password;
}