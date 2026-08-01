using DVLD.API.DTOs.Users;
using FluentValidation;

namespace DVLD.API.Validators.Users;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(u => u.UserName)
            .NotEmpty()
            .WithMessage("Username is required")
            .MaximumLength(50)
            .WithMessage("Username must not exceed 50 characters")
            .MinimumLength(4)
            .WithMessage("Username must not be less than 4 characters long")
            .Matches("^[a-zA-Z0-9._]+$")
            .WithMessage("Username may only contain letters, numbers, dots, and underscores.");

        RuleFor(u => u.Password)
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