using AutoMapper;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.Mapping
{
    public class CreateEmailRequestToFilePreloadMapper : Mapper<CreateMailRequest, FilePreload>
    {
        private readonly IMapper _mapper;

        public CreateEmailRequestToFilePreloadMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateMailRequest, FilePreload>()
                    .ForMember(dest => dest.EntryId, opt => opt.MapFrom(src => src.MailEntryId))
                    .ForMember(dest => dest.EwsId, opt => opt.MapFrom(src => src.MailEwsId))
                    .ForMember(dest => dest.EmailSubject, opt => opt.MapFrom(src => src.MailSubject));
            });

            _mapper = config.CreateMapper();
        }

        public override FilePreload Map(CreateMailRequest value)
        {
            return _mapper.Map<FilePreload>(value);
        }
    }
}