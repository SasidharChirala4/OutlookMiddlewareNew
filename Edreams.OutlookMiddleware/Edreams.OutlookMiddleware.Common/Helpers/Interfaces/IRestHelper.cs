using Edreams.Contracts.Data.Common;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.Common.Helpers.Interfaces
{
    public interface IRestHelper<T>
    {
        Task<HttpStatusCode> CheckState(string resourceUrl);

        Task<string> GetContent(string resourceUrl, bool skipResponseCheck = false);

        Task<List<T>> GetData(string resourceUrl, bool skipResponseCheck=false);

        Task<int> CountData(string resourceUrl, bool skipResponseCheck=false);

        Task<List<T>> GetData(string resourceUrl, RestParameter rsParameter, bool skipResponseCheck=false);

        Task<List<T>> GetData(string resourceUrl, List<RestParameter> parameters, bool skipResponseCheck=false);

        Task<ApiResult<T>> CreateNew(string resourceUrl, T objectToCreate, bool skipResponseCheck=false);

        Task<ApiResult<T>> CreateBulkNew(string resourceUrl, List<T> objectsToCreate, bool skipResponseCheck=false);

        Task<ApiResult<T>> CreateNew(string resourceUrl, T objectToCreate, RestParameter parameter, bool skipResponseCheck=false);

        Task<List<T>> CreateNew(string resourceUrl, bool skipResponseCheck=false);

        Task<string> CreateNewTransaction(string resourceUrl, bool skipResponseCheck=false);

        void ForceServiceToRunOverTls12();

        Task<ApiResult<T>> Update(string resourceUrl, RestParameter parameter, T objectToUpdate, bool skipResponseCheck=false);

        Task<ApiResult<T>> Patch(string resourceUrl, RestParameter parameter, T objectToUpdate, bool skipResponseCheck=false);

        Task<ApiResult<T>> Update(string resourceUrl, List<RestParameter> parameters, T objectToUpdate, bool skipResponseCheck=false);

        Task<ApiResult<T>> Patch(string resourceUrl, List<RestParameter> parameters, T objectToUpdate, bool skipResponseCheck=false);
        Task<bool> Delete(string resourceUrl, RestParameter parameter, bool skipResponseCheck=false);

        Task<bool> Delete(string resourceUrl, List<RestParameter> parameters, bool skipResponseCheck=false);
    }
}
