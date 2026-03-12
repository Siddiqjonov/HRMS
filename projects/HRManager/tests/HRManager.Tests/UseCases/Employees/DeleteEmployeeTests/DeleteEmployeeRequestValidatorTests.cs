using HrManager.Application.UseCases.Employees.DeleteEmployee;

namespace HRManager.Tests.UseCases.Employees.DeleteEmployeeTests;

public class DeleteEmployeeRequestValidatorTests
{
    private readonly DeleteEmployeeRequestValidator _validator = new();


    [Fact]
    public void Should_Have_Error_When_Id_Is_Empty()
    {
        // Arrange
        var request = new DeleteEmployeeRequest(Guid.Empty);

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
    }


    [Fact]
    public void Should_Pass_When_Id_Is_Valid()
    {
        // Arrange
        var request = new DeleteEmployeeRequest(Guid.NewGuid());

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }
}
