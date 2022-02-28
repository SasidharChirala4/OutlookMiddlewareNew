using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edreams.Common.DataAccess.Interfaces;
using Edreams.Common.Exceptions.Constants;
using Edreams.Common.Exceptions.Factories.Interfaces;
using Edreams.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Transactions.Interfaces;
using Edreams.OutlookMiddleware.Common.Constants;
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
        private readonly IRepository<EmailRecipient> _emailRecipientRepository;
        private readonly IRepository<Batch> _batchRepository;
        private readonly ISecurityContext _securityContext;

        public EmailManager(
            IRepository<Email> emailRepository,
            IRepository<FilePreload> preloadedFilesRepository,
            IMapper<CreateMailRequest, FilePreload> createEmailRequestToFilePreloadMapper,
            ITransactionHelper transactionHelper, IExceptionFactory exceptionFactory, IRepository<EmailRecipient> emailRecipientRepository,
            IRepository<Batch> batchRepository, ISecurityContext securityContext)
        {
            _emailRepository = emailRepository;
            _preloadedFilesRepository = preloadedFilesRepository;
            _createEmailRequestToFilePreloadMapper = createEmailRequestToFilePreloadMapper;
            _transactionHelper = transactionHelper;
            _exceptionFactory = exceptionFactory;
            _emailRecipientRepository = emailRecipientRepository;
            _batchRepository = batchRepository;
            _securityContext = securityContext;
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
                preloadedFile.InternetMessageId = request.InternetMessageId;
                preloadedFile.FileExtension = ".msg";
                preloadedFile.PrincipalName = preloadedFile.PrincipalName;
                preloadedFile = await _preloadedFilesRepository.Create(preloadedFile);

                CreateMailResponse response = new CreateMailResponse
                {
                    CorrelationId = _securityContext.CorrelationId,
                    EmailId = preloadedFile.EmailId,
                    FileId = preloadedFile.Id,
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
                    preloadedAttachment.FileExtension = attachment.Extension;
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

        /// <summary>
        /// GetEmails by batchId
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns>Emails of the batchId</returns>
        public async Task<IList<Email>> GetEmails(Guid batchId)
        {
            // Fetch the batch with specified unique ID.
            Batch batch = await _batchRepository.GetSingle(x => x.Id == batchId);

            // Throw an exception if a batch with specified unique ID cannot be found.
            if (batch == null)
            {
                throw _exceptionFactory.CreateEdreamsExceptionFromCode(EdreamsOutlookMiddlewareExceptionCode.OutlookMiddlewareBatchNotFound);
            }

            // Get a list of all related email's.
            IList<Email> emails = await _emailRepository.Find(
                x => x.Batch.Id == batchId);

            return emails;
        }

        /// <summary>
        /// Get email recipients by emailId
        /// </summary>
        /// <param name="emailId"></param>
        /// <returns>EmailRecipients</returns>
        public async Task<IList<EmailRecipient>> GetEmailRecipients(Guid emailId)
        {
            // Fetch all emails recipients that are related to the specified emails.
            IList<EmailRecipient> emailRecipients = await _emailRecipientRepository.Find(x => x.Email.Id == emailId, incl => incl.Email);

            return emailRecipients;
        }

        public async Task UpdateEmailStatus(Guid emailId, EmailStatus status)
        {
            Email email = await _emailRepository.GetSingle(x => x.Id == emailId);

            if (email == null)
            {
                throw _exceptionFactory.CreateEdreamsExceptionFromCode(EdreamsExceptionCode.UnknownFault);
            }

            email.Status = status;

            await _emailRepository.Update(email);
        }

        /// <summary>
        /// Updates the internetmessageid, ewsid to the sent email.
        /// </summary>
        /// <param name="emailId">Email id</param>
        /// <param name="internetMessageId">InternetMessageId</param>
        /// <param name="ewsId">ewsId</param>
        /// <returns></returns>
        public async Task UpdateEmailInternetMessageId(Guid emailId, string internetMessageId, string ewsId)
        {
            Email email = await _emailRepository.GetSingle(x => x.Id == emailId);

            if (email == null)
            {
                throw _exceptionFactory.CreateEdreamsExceptionFromCode(EdreamsExceptionCode.UnknownFault);
            }

            email.InternetMessageId = internetMessageId;
            email.EwsId = ewsId;

            await _emailRepository.Update(email);
        }
    }
}