using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edreams.Common.DataAccess.Interfaces;
using Edreams.Common.Exceptions;
using Edreams.Common.Exceptions.Factories;
using Edreams.Common.Exceptions.Factories.Interfaces;
using Edreams.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Transactions;
using Edreams.OutlookMiddleware.BusinessLogic.Transactions.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.DataTransferObjects.Api.Specific;
using Edreams.OutlookMiddleware.Mapping;
using Edreams.OutlookMiddleware.Model;
using Edreams.OutlookMiddleware.Tests.Framework.Extensions;
using FluentAssertions;
using Moq;
using Xunit;

namespace Edreams.OutlookMiddleware.Tests.BusinessLogic
{
    public class EmailManagerTests
    {
        #region <| CreateMail |>
        [Fact]
        public async Task EmailManager_CreateMail_Should_Create_Email()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var emailRepositoryMock = new Mock<IRepository<Email>>();
            var preloadedFilesRepositoryMock = new Mock<IRepository<FilePreload>>();
            var createEmailRequestToFilePreloadMapper = new CreateEmailRequestToFilePreloadMapper();
            var transactionHelperMock = new Mock<ITransactionHelper>();
            var exceptionFactoryMock = new Mock<IExceptionFactory>();
            var emailRecipientRepositoryMock = new Mock<IRepository<EmailRecipient>>();
            var batchRepositoryMock = new Mock<IRepository<Batch>>();
            var securityContextMock = new Mock<ISecurityContext>();

            // Create an instance of the "EmailManager" using the mocked dependencies.
            IEmailManager emailManager = new EmailManager(
                emailRepositoryMock.Object, preloadedFilesRepositoryMock.Object, createEmailRequestToFilePreloadMapper, transactionHelperMock.Object, exceptionFactoryMock.Object, emailRecipientRepositoryMock.Object, batchRepositoryMock.Object, securityContextMock.Object);

            // Prepare a request to use for when calling the "CreateMail" method.
            CreateMailRequest createEmailRequest = new CreateMailRequest()
            {
                Attachments = new List<Attachment>()
                {
                new Attachment()
                {
                    Name = "TaxDoc.doc"
                },
                new Attachment()
                {
                    Name = "TaxGeneral.doc"
                }
                },
                BatchId = new Guid(),
                MailSubject = "Mail to be saved in eDreams"
            };
            Mock<TransactionScope> transactionScopeMock = new Mock<TransactionScope>();
            #endregion

            #region [ MOCK ]
            // Mock the Transaction helper to mock the createscope method and returns mock trasaction scope object
            transactionHelperMock.Setup(x => x.CreateScope()).Returns(transactionScopeMock.Object);
            // Mock the "Create" method on the "Repository" and run the predicate lambda expression on
            // an  list of "FilePreload" object.
            preloadedFilesRepositoryMock.Setup(x => x.Create(It.IsAny<FilePreload>())).ReturnsAsync(new FilePreload() { Id = new Guid("9FC3D8F8-2A11-4B39-86A8-BD73D7FA3316") });
            #endregion

            #region [ ACT ]

            // Call the "CreateMail" method using the prepared request.
            CreateMailResponse response = await emailManager.CreateMail(createEmailRequest);

            #endregion

            #region [ ASSERT ]

            // The "CreateMail" method should not be Null.
            response.Should().NotBeNull();
            // The respose object should contains 2 attachments
            response.Attachments.Count.Should().Be(2);

            #endregion
        }
        #endregion

        #region <| GetEmails |>
        [Fact]
        public async Task EmailManager_GetEmails_Should_Validate_Batch_Id()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var emailRepositoryMock = new Mock<IRepository<Email>>();
            var preloadedFilesRepositoryMock = new Mock<IRepository<FilePreload>>();
            var createEmailRequestToFilePreloadMapper = new CreateEmailRequestToFilePreloadMapper();
            var transactionHelperMock = new Mock<TransactionHelper>();
            var exceptionFactory = new ExceptionFactory();
            var emailRecipientRepositoryMock = new Mock<IRepository<EmailRecipient>>();
            var batchRepositoryMock = new Mock<IRepository<Batch>>();
            var securityContextMock = new Mock<ISecurityContext>();

            // Create an instance of the "EmailManager" using the mocked dependencies.
            IEmailManager emailManager = new EmailManager(
                emailRepositoryMock.Object, preloadedFilesRepositoryMock.Object, createEmailRequestToFilePreloadMapper, transactionHelperMock.Object, exceptionFactory, emailRecipientRepositoryMock.Object, batchRepositoryMock.Object, securityContextMock.Object);

            // Prepare a request to use for when calling the "GetEmails" method.
            Guid batchId = new Guid();
            Batch batch = new Batch()
            {
                Id = new Guid("925b4ad2-d815-48a8-8f6f-b4b951b9c5d8"),
                Status = Enums.BatchStatus.Partially
            };
            List<Batch> batchDetails = new List<Batch>() { batch };

            #endregion

            #region [ MOCK ]
            // Mock the "GetSingle" method on the "Repository" and run the predicate lambda expression on
            // an  list of "batch" objects.
            batchRepositoryMock.SetupRepositoryGetSingle(batchDetails);
            #endregion

            #region [ ACT ]

            // Call the "GetEmails" method using the prepared request.
            EdreamsException exception = await Assert.ThrowsAsync<EdreamsException>(() => emailManager.GetEmails(batchId));

            #endregion

            #region [ ASSERT ]

            // The "GetEmails" method should throw exception when batchid is not exists.
            exception.Should().NotBeNull();

            #endregion
        }

        [Fact]
        public async Task EmailManager_GetEmails_Should_Get_Emails_For_BatchId()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var emailRepositoryMock = new Mock<IRepository<Email>>();
            var preloadedFilesRepositoryMock = new Mock<IRepository<FilePreload>>();
            var createEmailRequestToFilePreloadMapper = new CreateEmailRequestToFilePreloadMapper();
            var transactionHelperMock = new Mock<TransactionHelper>();
            var exceptionFactory = new ExceptionFactory();
            var emailRecipientRepositoryMock = new Mock<IRepository<EmailRecipient>>();
            var batchRepositoryMock = new Mock<IRepository<Batch>>();
            var securityContextMock = new Mock<ISecurityContext>();

            // Create an instance of the "EmailManager" using the mocked dependencies.
            IEmailManager emailManager = new EmailManager(
                emailRepositoryMock.Object, preloadedFilesRepositoryMock.Object, createEmailRequestToFilePreloadMapper, transactionHelperMock.Object, exceptionFactory, emailRecipientRepositoryMock.Object, batchRepositoryMock.Object, securityContextMock.Object);

            // Prepare a request to use for when calling the "GetEmails" method.
            Guid batchId = new Guid("925b4ad2-d815-48a8-8f6f-b4b951b9c5d8");
            Batch batch = new Batch()
            {
                Id = batchId,
                Status = Enums.BatchStatus.Partially
            };
            List<Batch> batchDetails = new List<Batch>() { batch };
            Email email = new Email()
            {
                Batch = new Batch() { Id = batchId },
                Files = new List<File>() { new File() { OriginalName = "TaxDocument.doc" } },
                InternetMessageId = "<b267a11ab00348e3aa77bd20e20e1060>"
            };
            List<Email> emails = new List<Email>() { email };
            #endregion

            #region [ MOCK ]
            // Mock the "GetSingle" method on the "Repository" and run the predicate lambda expression on
            // an  list of "Batch" objects.
            batchRepositoryMock.SetupRepositoryGetSingle(batchDetails);
            // Mock the "Find" method on the "Repository" and run the predicate lambda expression on
            // an  list of "Email" objects.
            emailRepositoryMock.SetupRepositoryFind(emails);
            #endregion

            #region [ ACT ]

            // Call the "CreateMail" method using the prepared request.
            IList<Email> response = await emailManager.GetEmails(batchId);

            #endregion

            #region [ ASSERT ]

            // The "GetEmails" method should return values when valid batchid is provided.
            response.Should().NotBeNull();
            // Validate the response to include emails
            response.Count().Should().Be(1);
            #endregion
        }
        #endregion

        #region <| GetEmailRecipients |>
        [Fact]
        public async Task EmailManager_GetEmailRecipients_Should_Return_EmailRecipients()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var emailRepositoryMock = new Mock<IRepository<Email>>();
            var preloadedFilesRepositoryMock = new Mock<IRepository<FilePreload>>();
            var createEmailRequestToFilePreloadMapper = new CreateEmailRequestToFilePreloadMapper();
            var transactionHelperMock = new Mock<TransactionHelper>();
            var exceptionFactory = new ExceptionFactory();
            var emailRecipientRepositoryMock = new Mock<IRepository<EmailRecipient>>();
            var batchRepositoryMock = new Mock<IRepository<Batch>>();
            var securityContextMock = new Mock<ISecurityContext>();

            // Create an instance of the "EmailManager" using the mocked dependencies.
            IEmailManager emailManager = new EmailManager(
                emailRepositoryMock.Object, preloadedFilesRepositoryMock.Object, createEmailRequestToFilePreloadMapper, transactionHelperMock.Object, exceptionFactory, emailRecipientRepositoryMock.Object, batchRepositoryMock.Object, securityContextMock.Object);

            // Prepare a request to use for when calling the "GetEmailRecipients" method.
            Guid emailId = new Guid();
            Email email = new Email()
            {
                Id = emailId,
                Batch = new Batch() { Id = new Guid() },
                Files = new List<File>() { new File() { OriginalName = "TaxDocument.doc" } },
                InternetMessageId = "<b267a11ab00348e3aa77bd20e20e1060>"
            };
            EmailRecipient emailRecipient = new EmailRecipient()
            {
                Email = email,
                Recipient = "edreamstest@deloitte.com"
            };
            List<EmailRecipient> emailRecipients = new List<EmailRecipient>() { emailRecipient };
            #endregion

            #region [ MOCK ]
            // Mock the "Find" method on the "Repository" and run the predicate lambda expression on
            // an  list of "categorizationResult" objects.
            emailRecipientRepositoryMock.SetupRepositoryFind(emailRecipients, x => x.Email);
            #endregion

            #region [ ACT ]

            // Call the "GetEmailRecipients" method using the prepared request.
            IList<EmailRecipient> response = await emailManager.GetEmailRecipients(emailId);

            #endregion

            #region [ ASSERT ]

            // The "GetEmailRecipients" method should return email recipients for corresponding emailid
            response.Should().NotBeNull();
            // The shold contain one emailrecipients.
            response.Count().Should().Be(1);
            #endregion
        }
        #endregion

        #region <| UpdateEmailStatus |>
        [Fact]
        public async Task EmailManager_UpdateEmailStatus_WithOut_Email_Exists_Should_Throw_Exception()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var emailRepositoryMock = new Mock<IRepository<Email>>();
            var preloadedFilesRepositoryMock = new Mock<IRepository<FilePreload>>();
            var createEmailRequestToFilePreloadMapper = new CreateEmailRequestToFilePreloadMapper();
            var transactionHelperMock = new Mock<TransactionHelper>();
            var exceptionFactory = new ExceptionFactory();
            var emailRecipientRepositoryMock = new Mock<IRepository<EmailRecipient>>();
            var batchRepositoryMock = new Mock<IRepository<Batch>>();
            var securityContextMock = new Mock<ISecurityContext>();

            // Create an instance of the "EmailManager" using the mocked dependencies.
            IEmailManager emailManager = new EmailManager(
                emailRepositoryMock.Object, preloadedFilesRepositoryMock.Object, createEmailRequestToFilePreloadMapper, transactionHelperMock.Object, exceptionFactory, emailRecipientRepositoryMock.Object, batchRepositoryMock.Object, securityContextMock.Object);

            // Prepare a request to use for when calling the "UpdateEmailStatus" method.
            Guid emailId = new Guid();
            Email email = new Email()
            {
                Id = emailId,
                Batch = new Batch() { Id = new Guid() },
                Files = new List<File>() { new File() { OriginalName = "TaxDocument.doc" } },
                InternetMessageId = "<b267a11ab00348e3aa77bd20e20e1060>"
            };
            List<Email> emails = new List<Email>() { email };
            EmailRecipient emailRecipient = new EmailRecipient()
            {
                Email = email,
                Recipient = "edreamstest@deloitte.com"
            };
            List<EmailRecipient> emailRecipients = new List<EmailRecipient>() { emailRecipient };
            #endregion

            #region [ MOCK ]
            // Mock the "Find" method on the "Repository" and run the predicate lambda expression on
            // an  list of "emails" objects.
            emailRepositoryMock.SetupRepositoryFind(emails);
            #endregion

            #region [ ACT ]

            // Call the "UpdateEmailStatus" method using the prepared request.
            EdreamsException exception = await Assert.ThrowsAsync<EdreamsException>(() => emailManager.UpdateEmailStatus(emailId, Enums.EmailStatus.Partially));

            #endregion

            #region [ ASSERT ]

            // The "UpdateEmailStatus" method should throw exception when emailid exists.
            exception.Should().NotBeNull();

            #endregion
        }

        [Fact]
        public async Task EmailManager_UpdateEmailStatus_Should_Update_Email_With_Status()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var emailRepositoryMock = new Mock<IRepository<Email>>();
            var preloadedFilesRepositoryMock = new Mock<IRepository<FilePreload>>();
            var createEmailRequestToFilePreloadMapper = new CreateEmailRequestToFilePreloadMapper();
            var transactionHelperMock = new Mock<TransactionHelper>();
            var exceptionFactory = new ExceptionFactory();
            var emailRecipientRepositoryMock = new Mock<IRepository<EmailRecipient>>();
            var batchRepositoryMock = new Mock<IRepository<Batch>>();
            var securityContextMock = new Mock<ISecurityContext>();

            // Create an instance of the "EmailManager" using the mocked dependencies.
            IEmailManager emailManager = new EmailManager(
                emailRepositoryMock.Object, preloadedFilesRepositoryMock.Object, createEmailRequestToFilePreloadMapper, transactionHelperMock.Object, exceptionFactory, emailRecipientRepositoryMock.Object, batchRepositoryMock.Object, securityContextMock.Object);

            // Prepare a request to use for when calling the "UpdateEmailStatus" method.
            Guid emailId = new Guid();
            Email email = new Email()
            {
                Id = emailId,
                Batch = new Batch() { Id = new Guid() },
                Files = new List<File>() { new File() { OriginalName = "TaxDocument.doc" } },
                InternetMessageId = "<b267a11ab00348e3aa77bd20e20e1060>"
            };
            List<Email> emails = new List<Email>() { email };

            #endregion

            #region [ MOCK ]
            // Mock the "GetSingle" method on the "Repository" and run the predicate lambda expression on
            // an  list of "email" objects.
            emailRepositoryMock.SetupRepositoryGetSingle(emails);
            #endregion

            #region [ ACT ]

            // Call the "UpdateEmailStatus" method using the prepared request.
            await emailManager.UpdateEmailStatus(emailId, Enums.EmailStatus.Partially);

            #endregion

            #region [ ASSERT ]

            // The EmailRepository's update method should be called once
            emailRepositoryMock.VerifyRepositoryUpdateSingle(Times.Once());
            #endregion
        }
        #endregion
    }
}