using AutoMapper;

namespace HrManager.Application.UseCases.Employees.UpdateEmployee;

public class UpdateEmployeeProfile : Profile
{
    public UpdateEmployeeProfile()
    {
        CreateMap<UpdateEmployeeRequest, Employee>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
