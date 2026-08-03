using DVLD.API.DTOs.TestAppointments;
using FluentValidation;

namespace DVLD.API.Validators.TestAppointments;

public class CreateTestAppointmentDtoValidator : AbstractValidator<CreateTestAppointmentDto>
{
    public CreateTestAppointmentDtoValidator()
    {
        RuleFor(x => x.AppointmentTime)
        .GreaterThanOrEqualTo(_ => DateTime.Now.AddMinutes(-1))
        .WithMessage("Appointment time cannot be in the past");
    }
}