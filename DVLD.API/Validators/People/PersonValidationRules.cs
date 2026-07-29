using DVLD.API.DTOs.People;
using FluentValidation;

namespace DVLD.API.Validators.People;

public static class PersonValidationRules
{
    public static void Apply<T>(AbstractValidator<T> validator)
        where T : class, IPersonDto
    {
        validator.RuleFor(p => p.NationalNo)
        .Cascade(CascadeMode.Stop)
        .NotEmpty()
        .Length(10);

        validator.RuleFor(p => p.FirstName)
        .Cascade(CascadeMode.Stop)
        .NotEmpty()
        .MaximumLength(50);

        validator.RuleFor(p => p.SecondName)
        .Cascade(CascadeMode.Stop)
        .NotEmpty()
        .MaximumLength(50);

        validator.RuleFor(p => p.ThirdName)
        .MaximumLength(50)
        .When(p => p.ThirdName != null);

        validator.RuleFor(p => p.LastName)
        .Cascade(CascadeMode.Stop)
        .NotEmpty()
        .MaximumLength(50);

        validator.RuleFor(p => p.DateOfBirth)
        .Cascade(CascadeMode.Stop)
        .NotEqual(default(DateOnly))
        .WithMessage("Date of birth is required.")
        .Must(BeAtLeast18YearsOld)
        .WithMessage("Person must be at least 18 years old");

        validator.RuleFor(p => p.GenderID)
        .GreaterThan(0)
        .WithMessage("Gender is required");

        validator.RuleFor(p => p.Address)
        .Cascade(CascadeMode.Stop)
        .NotEmpty()
        .MaximumLength(500);

        validator.RuleFor(p => p.Phone)
        .Cascade(CascadeMode.Stop)
        .NotEmpty()
        .MaximumLength(20);

        validator.RuleFor(p => p.Email)
        .EmailAddress()
        .When(p => !string.IsNullOrWhiteSpace(p.Email));

        validator.RuleFor(p => p.NationalityCountryID)
        .GreaterThan(0)
        .WithMessage("Nationality is required");
    }

    private static bool BeAtLeast18YearsOld(DateOnly dateOfBirth)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);

        var age = today.Year - dateOfBirth.Year;

        if (dateOfBirth > today.AddYears(-age))
            age--;

        return age >= 18;
    }
}
