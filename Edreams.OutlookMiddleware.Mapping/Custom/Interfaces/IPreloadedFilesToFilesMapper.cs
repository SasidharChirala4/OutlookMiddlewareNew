using System.Collections.Generic;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.Mapping.Custom.Interfaces
{
    public interface IPreloadedFilesToFilesMapper
    {
        IList<File> Map(Batch batch, IList<FilePreload> preloadedFiles, CommitBatchRequest request);
    }
}