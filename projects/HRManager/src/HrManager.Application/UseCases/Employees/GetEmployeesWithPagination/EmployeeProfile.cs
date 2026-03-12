using AutoMapper;

namespace HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;

public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        CreateMap<Employee, EmployeesBriefResponse>();
    }
}
