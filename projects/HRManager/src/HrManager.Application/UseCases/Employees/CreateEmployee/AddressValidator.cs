using HrManager.Domain.ValueObjects;

namespace HrManager.Application.UseCases.Employees.CreateEmployee;

public class AddressValidator : AbstractValidator<Address>
{
    public AddressValidator()
    {
        RuleFor(a => a.Region)
            .NotEmpty().WithMessage("Region is required")
            .MaximumLength(100).WithMessage("Region cannot exceed 100 characters");

        RuleFor(a => a.Street)
            .NotEmpty().WithMessage("Street is required")
            .MaximumLength(50).WithMessage("Street cannot exceed 50 characters");

        RuleFor(a => a.House)
            .NotEmpty().WithMessage("House number is required")
            .MaximumLength(20).WithMessage("House number cannot exceed 20 characters");

        RuleFor(a => a.Apartment)
            .MaximumLength(20).WithMessage("Apartment number cannot exceed 20 characters");

        RuleFor(a => a.FullAddress)
            .NotEmpty().WithMessage("Full address is required")
            .MaximumLength(250).WithMessage("Full address cannot exceed 250 characters");
    }
}