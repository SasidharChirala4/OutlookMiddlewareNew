using System.Collections.Generic;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.Mapping.Custom.Interfaces
{
    public interface IPreloadedFilesToFilesMapper
    {
        IList<File> Map(Batch batch, IList<FilePreload> preloadedFiles, EmailUploadOptions uploadOption, List<EmailRecipientDto> emailRecipients);
    }
}