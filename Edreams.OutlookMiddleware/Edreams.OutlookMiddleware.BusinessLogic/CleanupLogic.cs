using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.DataAccess.Repositories.Interfaces;
using Edreams.OutlookMiddleware.Model;
using Edreams.OutlookMiddleware.Model.Enums;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    public class CleanupLogic : ICleanupLogic
    {
        private readonly IRepository<FilePreload> _preloadedFilesRepository;

        public CleanupLogic(
            IRepository<FilePreload> preloadedFilesRepository)
        {
            _preloadedFilesRepository = preloadedFilesRepository;
        }

        public async Task<int> VerifyExpiration()
        {
            DateTime expirationDateTime = DateTime.UtcNow.AddMinutes(-1);
            var results = await _preloadedFilesRepository.Find(
                x => x.Status == EmailPreloadStatus.Pending && x.PreloadedOn < expirationDateTime);

            foreach (var file in results)
            {
                file.Status = EmailPreloadStatus.Expired;
            }

            await _preloadedFilesRepository.Update(results);

            return results.Count;
        }

        public async Task<int> Cleanup()
        {
            FilePreload preloadedFile = await _preloadedFilesRepository.GetFirst(x => x.Status != EmailPreloadStatus.Pending);
            if (preloadedFile != null)
            {
                return await _preloadedFilesRepository.RawSql("DELETE FROM PreloadedFiles WHERE BatchId = {0}", preloadedFile.BatchId);
            }

            return 0;
        }
    }
}