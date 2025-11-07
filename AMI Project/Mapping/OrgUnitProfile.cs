using AMI_Project.DTOs.OrgUnits;
using AMI_Project.Models;
using AutoMapper;

namespace AMI_Project.Mappings
{
    public class OrgUnitProfile : Profile
    {
        public OrgUnitProfile()
        {
            CreateMap<OrgUnit, OrgUnitReadDto>();
            CreateMap<OrgUnitCreateDto, OrgUnit>();
            CreateMap<OrgUnitUpdateDto, OrgUnit>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
