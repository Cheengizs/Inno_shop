using System.Data;
using FluentValidation;
using Users.Application.DTOs;
using Users.Domain.Constants;

namespace Users.Application.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(UserConstants.UserUsernameMinLength)
            .MaximumLength(UserConstants.UserUsernameMaxLength);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(UserConstants.UserPasswordMinLength)
            .MaximumLength(UserConstants.UserPasswordMaxLength);
        
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MinimumLength(UserConstants.UserFirstNameMinLength)
            .MaximumLength(UserConstants.UserFirstNameMaxLength);
        
        RuleFor(x => x.LastName)
            .NotEmpty()
            .MinimumLength(UserConstants.UserLastNameMinLength)
            .MaximumLength(UserConstants.UserLastNameMaxLength);
    }
}