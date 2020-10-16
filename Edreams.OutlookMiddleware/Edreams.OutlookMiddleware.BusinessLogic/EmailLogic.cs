using System;
using System.Threading.Tasks;
using System.Transactions;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.DataAccess.Repositories.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.DataTransferObjects.Api.Specific;
using Edreams.OutlookMiddleware.Mapping.Interfaces;
using Edreams.OutlookMiddleware.Model;
using Edreams.OutlookMiddleware.Model.Enums;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    public class EmailLogic : IEmailLogic
    {
        private readonly IRepository<FilePreload> _preloadedFilesRepository;
        private readonly IMapper<CreateMailRequest, FilePreload> _createEmailRequestToFilePreloadMapper;

        public EmailLogic(
            IRepository<FilePreload> preloadedFilesRepository,
            IMapper<CreateMailRequest, FilePreload> createEmailRequestToFilePreloadMapper)
        {
            _preloadedFilesRepository = preloadedFilesRepository;
            _createEmailRequestToFilePreloadMapper = createEmailRequestToFilePreloadMapper;
        }

        public async Task<CreateMailResponse> CreateMail(CreateMailRequest request)
        {
            using TransactionScope dbScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            FilePreload preloadedFile = _createEmailRequestToFilePreloadMapper.Map(request);
            preloadedFile.EmailId = Guid.NewGuid();
            preloadedFile.Kind = FileKind.Email;
            preloadedFile.PreloadedOn = DateTime.UtcNow;
            preloadedFile.Status = EmailPreloadStatus.Pending;
            preloadedFile = await _preloadedFilesRepository.Create(preloadedFile);

            CreateMailResponse response = new CreateMailResponse
            {
                CorrelationId = request.CorrelationId,
                FileId = preloadedFile.Id
            };

            foreach (var attachment in request.Attachments)
            {
                FilePreload preloadedAttachment = _createEmailRequestToFilePreloadMapper.Map(request);
                preloadedAttachment.EmailId = preloadedFile.EmailId;
                preloadedAttachment.Kind = FileKind.Attachment;
                preloadedAttachment.AttachmentId = attachment.Id;
                preloadedAttachment.PreloadedOn = DateTime.UtcNow;
                preloadedAttachment.Status = EmailPreloadStatus.Pending;
                preloadedAttachment = await _preloadedFilesRepository.Create(preloadedAttachment);

                response.Attachments.Add(new AttachmentResponse
                {
                    AttachmentId = attachment.Id,
                    FileId = preloadedAttachment.Id
                });
            }

            dbScope.Complete();

            return response;
        }
    }
}