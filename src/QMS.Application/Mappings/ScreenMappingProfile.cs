using AutoMapper;
using QMS.Application.DTOs.Screen;
using QMS.Core.Entities;

namespace QMS.Application.Mappings;

public class ScreenMappingProfile : Profile
{
    public ScreenMappingProfile()
    {
        // QMS_SCREEN mappings
        CreateMap<QMS_SCREEN, ScreenDto>()
            .ForMember(dest => dest.SectionName,
                opt => opt.MapFrom(src => src.Section != null ? src.Section.NAME : null));

        CreateMap<ScreenDto, QMS_SCREEN>()
            .ForMember(dest => dest.Section, opt => opt.Ignore())
            .ForMember(dest => dest.Queues, opt => opt.Ignore())
            .ForMember(dest => dest.BENHVIEN_ID, opt => opt.Ignore())
            .ForMember(dest => dest.NGUOITAO_ID, opt => opt.Ignore())
            .ForMember(dest => dest.NGUOICAPNHAT_ID, opt => opt.Ignore());
    }
}