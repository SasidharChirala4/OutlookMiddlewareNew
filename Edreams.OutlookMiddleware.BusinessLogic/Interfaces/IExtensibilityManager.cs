using System.Threading.Tasks;


namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    public interface IExtensibilityManager
    {
        /// <summary>
        /// Method to set suggested sites.
        /// </summary>
        /// <param name="from">The user to be added as suggestion.</param>
        /// <param name="siteUrl">Url of the site.</param>
        /// <param name="principalName">PrincipalName of the user.</param>
        /// <returns></returns>
        Task SetSuggestedSites(string from, string siteUrl, string principalName);
    }
}