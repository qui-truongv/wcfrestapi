using AutoMapper;
using Microsoft.Extensions.Logging;
using QMS.Application.Interfaces;
using QMS.Core.Interfaces;

namespace QMS.Application.Services;

public class SystemService : ISystemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<KioskService> _logger;

    public SystemService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<KioskService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public string GetValueOfParameter(string code, string defaultValue = "")
    {
        throw new NotImplementedException();
    }
}