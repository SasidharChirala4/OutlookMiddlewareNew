using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.Common.Exchange.Interfaces
{
    public interface IExchangeClient
    {
        Task<string> ResolveEmailAddress();
        Task<string> ResolveEmailAddress(string nameToResolve);
        Task<IList<string>> ExpandDistributionLists(string recipient);
    }
}