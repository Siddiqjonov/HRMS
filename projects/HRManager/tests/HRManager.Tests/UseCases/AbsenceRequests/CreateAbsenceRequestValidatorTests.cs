using FluentValidation.TestHelper;
using HrManager.Application.Common.Interfaces;
using HrManager.Application.UseCases.AbsenceRequests;
using HrManager.Domain.Enums;
using Moq;

namespace HRManager.Tests.UseCases.AbsenceRequests;

public class CreateAbsenceRequestValidatorTests
{
    private readonly Mock<IAbsenceBalanceService> _balanceServiceMock;
    private readonly CreateAbsenceRequestValidator _validator;

    public CreateAbsenceRequestValidatorTests()
    {
        _balanceServiceMock = new Mock<IAbsenceBalanceService>();

        _balanceServiceMock
            .Setup(x => x.HasOverlappingRequestAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _balanceServiceMock
            .Setup(x => x.HasSufficientBalanceAsync(
                It.IsAny<Guid>(),
                It.IsAny<RequestType>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _validator = new CreateAbsenceRequestValidator(_balanceServiceMock.Object);
    }

    private CreateAbsenceRequestRequest MakeValidRequest()
    {
        return new CreateAbsenceRequestRequest
        {
            EmployeeId = Guid.NewGuid(),
            RequestType = RequestType.Vacation,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date).AddDays(1),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date).AddDays(5),
            Reason = "Vacation request"
        };
    }

    [Fact]
    public async Task Should_Have_Error_When_EmployeeId_Is_Empty()
    {
        // Arrange
        var request = MakeValidRequest();
        request.EmployeeId = Guid.Empty;

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EmployeeId);
    }

    [Fact]
    public async Task Should_Have_Error_When_StartDate_Is_In_The_Past()
    {
        // Arrange
        var request = MakeValidRequest();
        request.StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date).AddDays(-1);

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StartDate);
    }

    [Fact]
    public async Task Should_Have_Error_When_EndDate_Before_StartDate()
    {
        // Arrange
        var request = MakeValidRequest();
        request.EndDate = request.StartDate.AddDays(-1);

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }

    [Fact]
    public async Task Should_Have_Error_When_Overlapping_Request_Exists()
    {
        // Arrange
        var request = MakeValidRequest();

        _balanceServiceMock.Setup(x => x.HasOverlappingRequestAsync(request.EmployeeId, request.StartDate, request.EndDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x).WithErrorMessage("There is already an overlapping request for this period.");
    }

    [Fact]
    public async Task Should_Have_Error_When_Insufficient_Balance()
    {
        // Arrange
        var request = MakeValidRequest();

        _balanceServiceMock.Setup(x => x.HasSufficientBalanceAsync(request.EmployeeId, request.RequestType, request.StartDate, request.EndDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _validator.TestValidateAsync(request);


        // Assert
        result.ShouldHaveValidationErrorFor(x => x).WithErrorMessage("Insufficient absence balance.");
    }


    [Fact]
    public async Task Should_Pass_When_Request_Is_Valid()
    {
        // Arrange
        var request = MakeValidRequest();

        // Act
        var result = await _validator.TestValidateAsync(request);


        // Assert
        result.ShouldNotHaveAnyValidationErrors();

    }
}
