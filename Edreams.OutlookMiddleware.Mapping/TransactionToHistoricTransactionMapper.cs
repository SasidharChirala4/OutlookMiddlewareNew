using AutoMapper;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.Mapping
{
    public class TransactionToHistoricTransactionMapper : Mapper<Transaction, HistoricTransaction>
    {
        private readonly IMapper _mapper;

        public TransactionToHistoricTransactionMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Transaction, HistoricTransaction>();
            });

            _mapper = config.CreateMapper();
        }

        public override HistoricTransaction Map(Transaction value)
        {
            return _mapper.Map<HistoricTransaction>(value);
        }
    }
}