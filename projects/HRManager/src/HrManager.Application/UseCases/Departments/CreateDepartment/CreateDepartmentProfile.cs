using AutoMapper;

namespace HrManager.Application.UseCases.Departments.CreateDepartment;

public class CreateDepartmentProfile : Profile
{
    public CreateDepartmentProfile()
    {
        CreateMap<CreateDepartmentRequest, Department>();
    }
}
