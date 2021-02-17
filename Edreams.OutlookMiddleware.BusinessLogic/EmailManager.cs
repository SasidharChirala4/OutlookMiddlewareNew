using System;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Transactions.Interfaces;
using Edreams.OutlookMiddleware.Common.Exceptions;
using Edreams.OutlookMiddleware.Common.Exceptions.Interfaces;
using Edreams.OutlookMiddleware.DataAccess.Repositories.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.DataTransferObjects.Api.Specific;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Mapping.Interfaces;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    public class EmailManager : IEmailManager
    {
        private readonly IRepository<Email> _emailRepository;
        private readonly IRepository<FilePreload> _preloadedFilesRepository;
        private readonly IMapper<CreateMailRequest, FilePreload> _createEmailRequestToFilePreloadMapper;
        private readonly ITransactionHelper _transactionHelper;
        private readonly IExceptionFactory _exceptionFactory;

        public EmailManager(
            IRepository<Email> emailRepository,
            IRepository<FilePreload> preloadedFilesRepository,
            IMapper<CreateMailRequest, FilePreload> createEmailRequestToFilePreloadMapper, 
            ITransactionHelper transactionHelper, IExceptionFactory exceptionFactory)
        {
            _emailRepository = emailRepository;
            _preloadedFilesRepository = preloadedFilesRepository;
            _createEmailRequestToFilePreloadMapper = createEmailRequestToFilePreloadMapper;
            _transactionHelper = transactionHelper;
            _exceptionFactory = exceptionFactory;
        }

        /// <summary>
        /// Method to create a Mail/ Attachment record in PreloadedFiles table
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<CreateMailResponse> CreateMail(CreateMailRequest request)
        {
            // Force a database transaction scope to make sure multiple
            // operations are combined as a single atomic operation.
            using (ITransactionScope transactionScope = _transactionHelper.CreateScope())
            {
                // Extract the EwsId, EntryId and MailSubject from the request and create the Mail
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

                // Extract the Attachment details of the Mail and insert in DB
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

                transactionScope.Commit();

                return response;
            }
        }

        public async Task UpdateEmailStatus(Guid emailId, EmailStatus status)
        {
            Email email = await _emailRepository.GetSingle(x => x.Id == emailId);

            if (email == null)
            {
                throw _exceptionFactory.CreateFromCode(EdreamsExceptionCode.UNKNOWN_FAULT);
            }

            email.Status = status;

            await _emailRepository.Update(email);
        }
    }
}