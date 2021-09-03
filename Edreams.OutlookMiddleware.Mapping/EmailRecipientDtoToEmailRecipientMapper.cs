using AutoMapper;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.Mapping
{
    public class EmailRecipientDtoToEmailRecipientMapper : Mapper<EmailRecipientDto, EmailRecipient>
    {

        private readonly IMapper _mapper;

        public EmailRecipientDtoToEmailRecipientMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EmailRecipientDto, EmailRecipient>().ForSourceMember(x => x.EmailId, opt => opt.DoNotValidate());
            });

            _mapper = config.CreateMapper();
        }

        public override EmailRecipient Map(EmailRecipientDto value)
        {
            return _mapper.Map<EmailRecipient>(value);
        }
    }

}
