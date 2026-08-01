using DVLD.API.DTOs.Users;
using FluentValidation;

namespace DVLD.API.Validators.Users;

public class ChangeUserNameDtoValidator : AbstractValidator<ChangeUserNameDto>
{
    public ChangeUserNameDtoValidator()
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
    }
}