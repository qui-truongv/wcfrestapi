using AutoMapper;
using QMS.Application.DTOs.Queue;
using QMS.Core.Entities;

namespace QMS.Application.Mappings;

public class QueueMappingProfile : Profile
{
    public QueueMappingProfile()
    {
        // QMS_QUEUE mappings
        CreateMap<QMS_QUEUE, QueueDto>()
            .ForMember(dest => dest.SectionName,
                opt => opt.MapFrom(src => src.Section != null ? src.Section.NAME : null))
            .ForMember(dest => dest.QueueTypeName,
                opt => opt.MapFrom(src => src.QueueType != null ? src.QueueType.NAME : null));

        // QMS_QUEUE_ITEM mappings
        CreateMap<QMS_QUEUE_ITEM, QueueItemDto>()
            .ForMember(dest => dest.QueueName,
                opt => opt.MapFrom(src => src.Queue != null ? src.Queue.NAME : null))
            .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => GetStateName(src.STATE)));

        CreateMap<CreateQueueItemDto, QMS_QUEUE_ITEM>()
            .ForMember(dest => dest.ID, opt => opt.Ignore())
            .ForMember(dest => dest.NGAYTAO, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.CREATEDATE, opt => opt.MapFrom(src =>
                src.NGAYCAPSTT.HasValue ? src.NGAYCAPSTT.Value.Date : DateTime.Today))
            .ForMember(dest => dest.CREATETIME, opt => opt.MapFrom(src =>
                src.NGAYCAPSTT ?? DateTime.Now));
    }

    private static string? GetStateName(int? state)
    {
        return state switch
        {
            0 => "Đang thực hiện",
            1 => "Đang chờ",
            2 => "Đã hoàn thành",
            3 => "Gọi nhỡ",
            4 => "Bỏ qua",
            -1 => "Đã hủy",
            100 => "Chưa xác định",
            _ => null
        };
    }
}