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
    public class EmailManager : IEmailManager
    {
        private readonly IRepository<FilePreload> _preloadedFilesRepository;
        private readonly IMapper<CreateMailRequest, FilePreload> _createEmailRequestToFilePreloadMapper;

        public EmailManager(
            IRepository<FilePreload> preloadedFilesRepository,
            IMapper<CreateMailRequest, FilePreload> createEmailRequestToFilePreloadMapper)
        {
            _preloadedFilesRepository = preloadedFilesRepository;
            _createEmailRequestToFilePreloadMapper = createEmailRequestToFilePreloadMapper;
        }

        /// <summary>
        /// Method to create an Email entry in the Preloaded Files
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<CreateMailResponse> CreateMail(CreateMailRequest request)
        {
            using TransactionScope dbScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            //Extract the EWSId, EntryId and MailSUbject from the request and create the Mail
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

            //Extract the Attachment details of the Mail from the request
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