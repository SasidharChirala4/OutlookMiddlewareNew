using System.Linq;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.Common.Exchange.Interfaces;
using Microsoft.Exchange.WebServices.Data;

namespace Edreams.OutlookMiddleware.Common.Exchange
{
    public class ExchangeClient : IExchangeClient
    {
        private readonly ExchangeService _exchangeService;
        private readonly ExchangeClientOptions _exchangeClientOptions;

        public ExchangeClient(
            ExchangeService exchangeService,
            ExchangeClientOptions exchangeClientOptions)
        {
            _exchangeService = exchangeService;
            _exchangeClientOptions = exchangeClientOptions;
        }

        public Task<string> ResolveEmailAddress()
        {
            return ResolveEmailAddress(_exchangeClientOptions.UserName);
        }

        public async Task<string> ResolveEmailAddress(string nameToResolve)
        {
            NameResolutionCollection nameResolutions = await _exchangeService.ResolveName(nameToResolve);

            if (nameResolutions.Count == 0)
            {
                return null;
            }

            NameResolution nameResolution = nameResolutions.First();

            if (nameResolution.Mailbox == null)
            {
                return null;
            }

            return nameResolution.Mailbox.Address;
        }
    }
}