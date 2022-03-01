using Edreams.Common.DataAccess.Interfaces;
using Edreams.Common.Logging.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edreams.Core.Common.Utilities;
using Aspose.Email;
using System.Text;
using Edreams.OutlookMiddleware.Enums;
using Edreams.Common.Exceptions;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    public class NotificationManager : INotificationManager
    {

        #region <| Private Memebers |>

        private readonly IRepository<Batch> _batchRepository;
        private readonly IRepository<Email> _emailRepository;
        private readonly IRepository<EmailNotification> _emailNotificationRepository;
        private readonly IEdreamsConfiguration _configuration;
        private readonly IEdreamsLogger<NotificationManager> _logger;

        #endregion

        #region <| Construction |>

        public NotificationManager(IRepository<Batch> batchRepository, IRepository<Email> emailRepository,
            IRepository<EmailNotification> emailNotificationRepository,
            IEdreamsConfiguration configuration, IEdreamsLogger<NotificationManager> logger)
        {
            _batchRepository = batchRepository;
            _emailRepository = emailRepository;
            _emailNotificationRepository = emailNotificationRepository;
            _configuration = configuration;
            _logger = logger;
        }

        #endregion

        #region <| Public Methods |>

        /// <summary>
        /// Create email notification
        /// </summary>
        /// <param name="batchId"
        /// <returns></returns>
        public async Task CreateNotification(Guid batchId)
        {
            // Fetch the batch with specified unique ID.
            Batch batch = await _batchRepository.GetSingle(x => x.Id == batchId);
            EmailNotification emailNotification = new EmailNotification
            {
                Batch = batch,
                NotificationSent = false
            };

            await _emailNotificationRepository.Create(emailNotification);
        }

        /// <summary>
        /// Process all the pending notifications
        /// </summary>
        /// <returns></returns>
        public async Task ProcessNotifications()
        {
            try
            {
                var pendingNotifications = await _emailNotificationRepository.Find(i => i.NotificationSent == false, incl => incl.Batch);
                if (pendingNotifications.Count > 0)
                {
                    _logger.LogInformation($"{pendingNotifications.Count} notifications are ready to process!");
                    await SendEmails(pendingNotifications.ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at processing notifications");
            }
        }

        #endregion

        #region <| Prvate Memebers |>

        private async Task SendEmails(List<EmailNotification> notifications)
        {
            try
            {
                EmailUtility emailUtility = new EmailUtility();
                foreach (EmailNotification notification in notifications)
                {
                    string body = await CreateHTMLBodyMessage(notification.Batch.Id, notification.Batch.UploadLocationFolder);

                    MailMessage message = new MailMessage
                    {
                        To = notification.Batch.PrincipalName,
                        From = _configuration.EmailOutgoingFromAddress,
                        Subject = _configuration.EmailErrorSubject,
                        HtmlBody = body
                    };

                    await emailUtility.SendEmail(message, _configuration.EmailOutgoingSmtpAddress);
                    notification.NotificationSent = true;
                    await _emailNotificationRepository.Update(notification);
                    _logger.LogInformation($"Notification sent to user {notification.Batch.PrincipalName} for batch:{notification.Batch.Id}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error at send emails");
                throw;
            }

        }

        /// <summary>
        /// Create HTML body message
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="uploadLocation"></param>
        /// <returns></returns>
        private async Task<string> CreateHTMLBodyMessage(Guid batchId, string uploadLocation)
        {
            IList<Email> emails = await _emailRepository.Find(x => x.Batch.Id == batchId && x.Status != EmailStatus.Successful, inc => inc.Files);
            if (emails.Count == 0)
            {
                throw new EdreamsException($"No failed emails found for batch:{batchId}");
            }

            string emailsSubject = string.Empty;
            string attachmentsSubject = string.Empty;

            foreach (Email email in emails)
            {
                // Upload failed files
                List<File> uploadFailedFiles = email.Files.Where(i => i.Kind == FileKind.Email && i.Status != FileStatus.Uploaded && i.ShouldUpload).ToList();
                foreach (File file in uploadFailedFiles)
                {
                    emailsSubject += file.OriginalName + "<br/>";
                }
                // Upload failed attachments
                List<File> uploadFailedAttachments = email.Files.Where(i => i.Kind == FileKind.Attachment && i.Status != FileStatus.Uploaded && i.ShouldUpload).ToList();
                foreach (File file in uploadFailedAttachments)
                {
                    attachmentsSubject += file.OriginalName + "<br/>";
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("<p style=\"font-size:11pt;font-family:Calibri\">Error uploading ");
            if (!string.IsNullOrEmpty(emailsSubject))
                sb.Append(" email(s) ");
            if (!string.IsNullOrEmpty(attachmentsSubject))
                sb.Append(" attachment(s) ");
            sb.Append(" into ");
            sb.Append(uploadLocation);
            if (!string.IsNullOrEmpty(emailsSubject))
                sb.Append($"<br><b>Email(s) Subject</b>: {emailsSubject}");
            if (!string.IsNullOrEmpty(attachmentsSubject))
                sb.Append($"<br><b>Attachment(s) Subject</b>: {attachmentsSubject}");
            sb.Append($"<br><b>Error</b>: {_configuration.ErrorMessage}");
            sb.Append("</p>");

            return sb.ToString();
        }

        #endregion
    }
}
