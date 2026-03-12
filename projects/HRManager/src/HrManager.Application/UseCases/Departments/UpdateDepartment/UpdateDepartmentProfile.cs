using AutoMapper;

namespace HrManager.Application.UseCases.Departments.UpdateDepartment;

public class UpdateDepartmentProfile : Profile
{
    public UpdateDepartmentProfile()
    {
        CreateMap<UpdateDepartmentRequest, Department>();
    }
}
