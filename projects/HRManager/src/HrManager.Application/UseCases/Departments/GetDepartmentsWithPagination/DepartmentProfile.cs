using AutoMapper;
using HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;

namespace HrManager.Application.UseCases.Departments.GetDepartmentsWithPagination;

public class DepartmentProfile : Profile
{
    public DepartmentProfile()
    {
        CreateMap<Department, DepartmentResponse>();
        CreateMap<Employee, EmployeesBriefResponse>();
    }
}
