using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.Common.Exchange.Interfaces;
using Microsoft.Exchange.WebServices.Data;

namespace Edreams.OutlookMiddleware.Common.Exchange
{
    public class ExchangeClient : IExchangeClient
    {
        private readonly ExchangeService _exchangeService;
        private readonly ExchangeClientToken _exchangeClientToken;
        private readonly ExchangeClientOptions _exchangeClientOptions;

        public ExchangeClient(
            ExchangeService exchangeService,
            ExchangeClientToken exchangeClientToken,
            ExchangeClientOptions exchangeClientOptions)
        {
            _exchangeService = exchangeService;
            _exchangeClientToken = exchangeClientToken;
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


        public async Task<IList<string>> ExpandDistributionLists(string recipient)
        {
            IList<string> listOfEmails = new List<string>();
            // Return the expanded group.
            ExpandGroupResults groupMembers = await _exchangeService.ExpandGroup(recipient);

            // Loop through the group members.
            foreach (EmailAddress address in groupMembers.Members)
            {
                // Check to see if the type is not a public group
                // From this level we will ignore nested groups.
                if (address.MailboxType != MailboxType.PublicGroup)
                {
                    listOfEmails.Add(address.Address);
                }
            }
            return listOfEmails;
        }
    }
}