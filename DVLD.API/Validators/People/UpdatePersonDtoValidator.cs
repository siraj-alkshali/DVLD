using DVLD.API.DTOs.People;
using FluentValidation;

namespace DVLD.API.Validators.People;

public class UpdatePersonDtoValidator : AbstractValidator<UpdatePersonDto>
{
    public UpdatePersonDtoValidator()
    {
        PersonValidationRules.Apply(this);
    }
}