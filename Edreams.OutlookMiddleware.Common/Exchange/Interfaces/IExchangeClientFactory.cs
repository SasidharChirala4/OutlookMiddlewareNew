using Microsoft.Exchange.WebServices.Data;
using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.Common.Exchange.Interfaces
{
    public interface IExchangeClientFactory
    {
        Task<IExchangeClient> AuthenticateAndCreateClient(ExchangeClientOptions clientOptions);
        Task<ExchangeService> AuthenticateAndCreateService(ExchangeClientOptions clientOptions);
    }
}