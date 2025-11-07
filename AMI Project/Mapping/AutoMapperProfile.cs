using AMI_Project.DTOs.Auth;
using AMI_Project.DTOs.Billing;
using AMI_Project.DTOs.Consumers;
using AMI_Project.DTOs.Meter;
using AMI_Project.DTOs.MeterReadings;
using AMI_Project.DTOs.OrgUnits;
using AMI_Project.DTOs.Tariffs;
using AMI_Project.DTOs.Users;
using AMI_Project.Models;
using AutoMapper;

namespace AMI_Project.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // -------------------------------------------------
            // 👤 USER
            // -------------------------------------------------
            CreateMap<User, UserReadDto>();
            CreateMap<UserCreateDto, User>();
            CreateMap<UserUpdateDto, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UserLoginDto, User>();
            CreateMap<User, UserAuthResponseDto>();

            // -------------------------------------------------
            // 🧩 ROLE
            // -------------------------------------------------
            CreateMap<Role, RoleReadDto>();
            CreateMap<RoleCreateDto, Role>();

            // -------------------------------------------------
            // 👥 CONSUMER
            // -------------------------------------------------
            CreateMap<Consumer, ConsumerReadDto>();
            CreateMap<ConsumerCreateDto, Consumer>();
            CreateMap<ConsumerUpdateDto, Consumer>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // -------------------------------------------------
            // ⚡ METER
            // -------------------------------------------------
            CreateMap<Meter, MeterReadDto>();
            CreateMap<MeterCreateDto, Meter>();
            CreateMap<MeterUpdateDto, Meter>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // -------------------------------------------------
            // 🔌 METER READING
            // -------------------------------------------------
            CreateMap<MeterReading, MeterReadingReadDto>();
            CreateMap<MeterReadingCreateDto, MeterReading>();
            CreateMap<MeterReadingUpdateDto, MeterReading>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // -------------------------------------------------
            // 🏢 ORG UNIT
            // -------------------------------------------------
            CreateMap<OrgUnit, OrgUnitReadDto>()
                .ForMember(dest => dest.ParentName,
                    opt => opt.MapFrom(src => src.Parent != null ? src.Parent.Name : null));
            CreateMap<OrgUnitCreateDto, OrgUnit>();
            CreateMap<OrgUnitUpdateDto, OrgUnit>()
                .ForAllMembers(opts =>
                    opts.Condition((src, dest, srcMember) => srcMember != null));
            // -------------------------------------------------
            // 💡 TARIFF & SLABS
            // -------------------------------------------------
            CreateMap<Tariff, TariffReadDto>();
            CreateMap<TariffCreateDto, Tariff>();
            CreateMap<TariffUpdateDto, Tariff>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // -------------------------------------------------
            // 💰 BILLING
            // -------------------------------------------------
            CreateMap<Bill, BillReadDto>();
            CreateMap<BillCreateDto, Bill>();
            CreateMap<BillDetail, BillDetailReadDto>();
            CreateMap<BillDetailCreateDto, BillDetail>();
        }
    }
}
