using AutoMapper;
using QMS.Application.DTOs.Kiosk;
using QMS.Core.Entities;

namespace QMS.Application.Mappings;

public class KioskMappingProfile : Profile
{
    public KioskMappingProfile()
    {
        // QMS_KIOSK mappings
        CreateMap<QMS_KIOSK, KioskDto>();

        CreateMap<KioskDto, QMS_KIOSK>()
            .ForMember(dest => dest.BENHVIEN_ID, opt => opt.Ignore())
            .ForMember(dest => dest.NGUOITAO_ID, opt => opt.Ignore())
            .ForMember(dest => dest.NGUOICAPNHAT_ID, opt => opt.Ignore());

        // QMS_KIOSK_QUEUE mappings
        CreateMap<QMS_KIOSK_QUEUE, KioskQueueDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.DISPLAYTEXT))
            .ForMember(dest => dest.QueueName, opt => opt.MapFrom(src => src.Queue != null ? src.Queue.NAME : ""))
            .ForMember(dest => dest.ScreenId, opt => opt.MapFrom(src => src.Queue != null ? src.Queue.SCREEN_ID ?? 0 : 0))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.PRIORITY ?? 0))
            .ForMember(dest => dest.DisplayOrder, opt => opt.MapFrom(src => src.DisplayOrder));
    }
}