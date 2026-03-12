namespace HrManager.Application.UseCases.Employees.CreateEmployee;

public class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest>
{
    public CreateEmployeeRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50);

        RuleFor(x => x.MiddleName)
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .MaximumLength(50)
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.PassportNumber)
            .NotEmpty().WithMessage("Passport number is required")
            .MaximumLength(20);

        RuleFor(x => x.DateOfBirth)
            .Must(date => date < DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Date of birth must be in the past.");

        RuleFor(x => x.Nationality)
            .NotEmpty().WithMessage("Nationality is required")
            .MaximumLength(50);

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Invalid gender value");

        RuleFor(x => x.Pinfl)
            .NotEmpty().WithMessage("PINFL is required")
            .MaximumLength(14);

        RuleFor(x => x.PensionFundNumber)
            .MaximumLength(20);

        RuleFor(x => x.TaxIdentificationNumber)
            .MaximumLength(20);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20);

        RuleFor(x => x.HireDate)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Hire date cannot be in the future");

        RuleFor(x => x.Address)
            .NotNull().WithMessage("Address is required")
            .SetValidator(new AddressValidator());

        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Department is required");

        RuleFor(x => x.PositionId)
            .NotEmpty().WithMessage("Position is required");

        RuleFor(x => x.Salary)
            .GreaterThan(0).WithMessage("Salary must be greater than zero");

        RuleFor(x => x.ScheduleId)
            .NotEmpty().WithMessage("Schedule is required");
    }
}