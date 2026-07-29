using DVLD.API.DTOs.People;
using FluentValidation;

namespace DVLD.API.Validators.People;

public class CreatePersonDtoValidator : AbstractValidator<CreatePersonDto>
{
    public CreatePersonDtoValidator()
    {
        PersonValidationRules.Apply(this);
    }
}