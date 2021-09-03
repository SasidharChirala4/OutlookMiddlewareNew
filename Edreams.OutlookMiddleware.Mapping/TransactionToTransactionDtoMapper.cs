using AutoMapper;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.Mapping
{
    public class TransactionToTransactionDtoMapper : Mapper<Transaction, TransactionDto>
    {
        private readonly IMapper _mapper;

        public TransactionToTransactionDtoMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Transaction, TransactionDto>();
            });

            _mapper = config.CreateMapper();
        }

        public override TransactionDto Map(Transaction value)
        {
            return _mapper.Map<TransactionDto>(value);
        }
    }
}