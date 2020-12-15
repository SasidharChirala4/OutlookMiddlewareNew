using Edreams.Contracts.Data.Logging;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using AutoMapper;

namespace Edreams.OutlookMiddleware.Mapping
{
    class RecordLogRequestToLogEntryMapper : Mapper<RecordLogRequest, LogEntry>
    {
        private readonly IMapper _mapper;

        public RecordLogRequestToLogEntryMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RecordLogRequest, LogEntry>()
                    .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level))
                    .ForMember(dest => dest.Component, opt => opt.MapFrom(src => src.Component))
                    .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
                    .ForMember(dest => dest.ExceptionDetails, opt => opt.MapFrom(src => src.ExceptionDetails))
                    .ForMember(dest => dest.ExceptionType, opt => opt.MapFrom(src => src.ExceptionType))
                    .ForMember(dest => dest.ExecutionStep, opt => opt.MapFrom(src => src.ExecutionStep))
                    .ForMember(dest => dest.MethodPath, opt => opt.MapFrom(src => src.MethodPath))
                    .ForMember(dest => dest.ProductCorrelation, opt => opt.MapFrom(src => src.CorrelationId));
            });

            _mapper = config.CreateMapper();
        }

        public override LogEntry Map(RecordLogRequest value)
        {
            return _mapper.Map<LogEntry>(value);
        }
    }
}
