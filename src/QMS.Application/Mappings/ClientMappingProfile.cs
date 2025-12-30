using AutoMapper;
using QMS.Application.DTOs.Client;
using QMS.Core.Entities;

namespace QMS.Application.Mappings;

public class ClientMappingProfile : Profile
{
    public ClientMappingProfile()
    {
        CreateMap<QMS_CLIENT, ClientDto>()
            .ForMember(dest => dest.QueueName,
                opt => opt.MapFrom(src => src.Queue != null ? src.Queue.NAME : null));

        CreateMap<ClientDto, QMS_CLIENT>()
            .ForMember(dest => dest.Queue, opt => opt.Ignore())
            .ForMember(dest => dest.BENHVIEN_ID, opt => opt.Ignore())
            .ForMember(dest => dest.NGUOITAO_ID, opt => opt.Ignore())
            .ForMember(dest => dest.NGUOICAPNHAT_ID, opt => opt.Ignore());
    }
}