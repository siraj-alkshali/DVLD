using DVLD.API.DTOs.Users;
using FluentValidation;

namespace DVLD.API.Validators.Users;

public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(u => u.CurrentPassword)
        .NotEmpty()
        .WithMessage("Current password is required");

        RuleFor(u => u.NewPassword)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long")
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]")
            .WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one number")
            .Matches("[^a-zA-Z0-9]")
            .WithMessage("Password must contain at least one special character");
    }
}