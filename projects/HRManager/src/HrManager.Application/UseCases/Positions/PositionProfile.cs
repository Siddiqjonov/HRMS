using AutoMapper;
using HrManager.Application.UseCases.Positions.CreatePosition;
using HrManager.Application.UseCases.Positions.UpdatePosition;

namespace HrManager.Application.UseCases.Positions;

public class PositionProfile : Profile
{
    public PositionProfile()
    {
        CreateMap<Position, PositionDto>()
            .ForMember(
                dest => dest.DepartmentName,
                opt => opt.MapFrom(src => src.Department.Name));

        CreateMap<CreatePositionRequest, Position>();
        CreateMap<UpdatePositionRequest, Position>();
    }
}
