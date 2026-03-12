using FluentValidation.TestHelper;
using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;

namespace HRManager.Tests.UseCases.Employees.EmployeesWithPaginationTests;

public class GetEmployeesRequestValidatorTests
{

    private readonly GetEmployeesRequestValidator _validator = new();

    [Fact]
    public void Should_HaveError_WhenPageIsZero()
    {
        var model = new GetEmployeesWithPaginationRequest(pageNumber: 0, pageSize: 10);
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.pageNumber);
    }

    [Fact]
    public void Should_HaveError_WhenPageSizeIsTooBig()
    {
        var model = new GetEmployeesWithPaginationRequest(pageNumber: 1, pageSize: 200);
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.pageSize);
    }

    [Fact]
    public void Should_Pass_WhenValidRequest()
    {
        var model = new GetEmployeesWithPaginationRequest(pageNumber: 1, pageSize: 20);
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
