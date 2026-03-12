using AutoMapper;

namespace HrManager.Application.UseCases.Employees.GetEmployee
{
    public class GetEmployeeByIdProfile : Profile
    {
        public GetEmployeeByIdProfile()
        {
            CreateMap<Employee, EmployeeResponse>()
                 .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
                 .ForMember(dest => dest.PositionName, opt => opt.MapFrom(src => src.Position.Title));
        }
    }
}
