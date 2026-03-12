namespace HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;

public class EmployeesBriefResponse
{
    public Guid Id { get; set; }

    public string FullName { get; set; }

    public string DepartmentName { get; set; }

    public string PositionName { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public DateOnly HireDate { get; set; }

    public bool IsManagerOfDepartment { get; set; }
}