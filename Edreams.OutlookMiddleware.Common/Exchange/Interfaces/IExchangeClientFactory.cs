using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.Common.Exchange.Interfaces
{
    public interface IExchangeClientFactory
    {
        Task<IExchangeClient> AuthenticateAndCreateClient(ExchangeClientOptions clientOptions);
    }
}