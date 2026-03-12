using AutoMapper;

namespace HrManager.Application.UseCases.Employees.CreateEmployee;

public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        CreateMap<CreateEmployeeRequest, Employee>();
    }
}