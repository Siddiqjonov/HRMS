using AutoMapper;

namespace HrManager.Application.UseCases.Departments.GetDepartment;

public class GetDepartmentByIdProfile : Profile
{
    public GetDepartmentByIdProfile()
    {
        CreateMap<Department, DepartmentDetailsResponse>();
    }
}
