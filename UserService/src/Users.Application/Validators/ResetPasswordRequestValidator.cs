using FluentValidation;
using Users.Application.DTOs;
using Users.Domain.Constants;

namespace Users.Application.Validators;

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(UserConstants.UserPasswordMinLength).WithMessage($"Password must be at least {UserConstants.UserPasswordMinLength} characters long.")
            .MaximumLength(UserConstants.UserPasswordMaxLength).WithMessage($"Password must be max {UserConstants.UserPasswordMaxLength} characters long.");

        RuleFor(x => x.ConfirmNewPassword)
            .Equal(x => x.NewPassword).WithMessage("Passwords do not match.");
    }
    
}