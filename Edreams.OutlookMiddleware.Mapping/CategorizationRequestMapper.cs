using CategorizationEntity = Edreams.OutlookMiddleware.Model.CategorizationRequest;
using CategorizationContract = Edreams.OutlookMiddleware.DataTransferObjects.Api.CategorizationRequest;
using AutoMapper;

namespace Edreams.OutlookMiddleware.Mapping
{
    public class CategorizationRequestMapper : Mapper<CategorizationEntity, CategorizationContract>
    {
        private readonly IMapper _mapper;

        public CategorizationRequestMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {

                cfg.CreateMap<CategorizationEntity, CategorizationContract>()
                    .ForMember(dest => dest.CategorizationRequestType, opt => opt.MapFrom(src => src.Type));
            });

            _mapper = config.CreateMapper();
        }

        public override CategorizationContract Map(CategorizationEntity value)
        {
            return _mapper.Map<CategorizationContract>(value);
        }
    }
}
