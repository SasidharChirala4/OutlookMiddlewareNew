using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Edreams.Outlook.TestPlugin.Helpers;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.Enums;
using Microsoft.Office.Interop.Outlook;
using Edreams.OutlookMiddleware.DataTransferObjects;

namespace Edreams.Outlook.TestPlugin.Views
{
    public partial class Dashboard : UserControl
    {
        private readonly SynchronizationContext _synchronizationContext;
        private List<MailItem> _selection;
        private List<Guid> _fileIds;
        private Guid? _batchId;
        private bool _preloaded;
        private bool _committed;
        public const string SchemaInternetMessageId = "http://schemas.microsoft.com/mapi/proptag/0x1035001F";
        public Dashboard()
        {
            InitializeComponent();
            _synchronizationContext = SynchronizationContext.Current;
        }

        public async Task UpdateSelection(List<MailItem> selection)
        {
            if (_preloaded && !_committed && _batchId.HasValue)
            {
                await HttpHelper.CancelBatch(_batchId.Value);
            }

            _synchronizationContext.Send(_ =>
            {

                selectionListBox.Items.Clear();
                _batchId = null;
                _selection = selection;
                _preloaded = false;
                _committed = false;
                preloadButton.Enabled = true;
                commitButton.Enabled = true;

                progressBar.Minimum = 0;
                progressBar.Maximum = _selection.Count;
                progressBar.Value = 0;

                foreach (MailItem mail in _selection)
                {
                    selectionListBox.Items.Add(mail.Subject);
                }

            }, null);
        }

        private async void preloadButton_Click(object sender, System.EventArgs e)
        {
            Guid batchId = Guid.NewGuid();
            Guid correlationId = Guid.NewGuid();
            _fileIds = new List<Guid>();
            preloadButton.Enabled = false;

            int currentSelected = 0;

            foreach (MailItem mail in _selection)
            {
                try
                {
                    currentSelected++;

                    string ewsId = await ExchangeHelper.ConvertEntryIdToEwsId(mail.EntryID);

                    var ewsEmail = await ExchangeHelper.DownloadEmail(ewsId);
                    var internetMessageId = mail.PropertyAccessor.GetProperty(SchemaInternetMessageId);

                    var createMailRequest = new CreateMailRequest
                    {
                        BatchId = batchId,
                        CorrelationId = correlationId,
                        MailEntryId = mail.EntryID,
                        MailEwsId = ewsId,
                        MailSubject = ewsEmail.Subject,
                        InternetMessageId = internetMessageId

                    };

                    foreach (var attachment in ewsEmail.Attachments)
                    {
                        createMailRequest.Attachments.Add(
                            new OutlookMiddleware.DataTransferObjects.Api.Specific.Attachment
                            {
                                Id = attachment.Id,
                                Name = attachment.Name
                            });
                    }

                    var createMailResponse = await HttpHelper.CreateMail(createMailRequest);
                    using (var memoryStream = new MemoryStream(ewsEmail.Data))
                    {
                        await HttpHelper.UploadAsync(memoryStream, mail.Subject, createMailResponse.FileId);
                        _fileIds.Add(createMailResponse.FileId);
                    }

                    foreach (var attachment in createMailResponse.Attachments)
                    {
                        var binary = ewsEmail.Attachments.Single(x => x.Id == attachment.AttachmentId);
                        using (var memoryStream = new MemoryStream(binary.Data))
                        {
                            await HttpHelper.UploadAsync(memoryStream, binary.Name, attachment.FileId);
                        }
                        _fileIds.Add(attachment.FileId);
                    }
                }
                catch
                {
                    // Nothing we can do...
                }
                finally
                {
                    _synchronizationContext.Send(value => { progressBar.Value = (int)value; }, currentSelected);
                }
            }

            _batchId = batchId;
            _preloaded = true;
            await Task.Delay(500);
            _synchronizationContext.Send(_ =>
            {
                progressBar.Value = progressBar.Maximum;
            }, null);
        }

        private async void commitButton_Click(object sender, EventArgs e)
        {
            if (_preloaded && _batchId.HasValue)
            {
                // TODO : Needs to adjust CommitBatchRequest Logic 
                CommitBatchRequest commitBatchRequest = new CommitBatchRequest()
                {
                    BatchId = new Guid("8A34B973-B5D8-4CFF-9769-2F648335EAD2"),
                    UploadOption = EmailUploadOptions.Emails,
                    //EmailRecipients = new List<EmailRecipientDto>() { new EmailRecipientDto() { EmailId=new Guid("E400B31C-BC09-4F4F-B16F-546181285D67") ,Type=EmailRecipientType.Contact,Recipient="kkaredla@deloitte.com" },
                    // new EmailRecipientDto() { EmailId=new Guid("7F6856BF-6490-4D60-A52B-FE1301C894CD") ,Type=EmailRecipientType.Contact,Recipient="bkonijeti@deloitte.com" }}
                };
                foreach (Guid fileId in _fileIds)
                {
                    commitBatchRequest.Files.Add(new FileDetailsDto()
                    {
                        NewName = "Test-Mail-NewName",
                        OriginalName = "Test-Mail-OriginalName",
                        Id = fileId,
                        Metadata = new List<MetadataDto>()
                        {
                            new MetadataDto(){PropertyName ="MailHeaders_MailCc",PropertyValue ="bkonijet@deloitte.com,kkaredla@deloitte.com"  },
                            new MetadataDto(){PropertyName ="MailHeaders_MailTo",PropertyValue ="kkaredla@deloitte.com"  },
                            new MetadataDto(){PropertyName ="dttedrMailSubject",PropertyValue ="RE: Solenis: Sigura Water and Platinum Equity Merger - Response Requested"  },
                            new MetadataDto(){PropertyName ="dttedrMailMessageId",PropertyValue ="DB9PR85MB029776038DE1D4E9A672E170D4F19@DB9PR85MB0297.NAMPRD85.PROD.OUTLOOK.COM"  },
                            new MetadataDto(){PropertyName ="dttedrMailCategory",PropertyValue ="Received Mail"  },
                            new MetadataDto(){PropertyName ="dttedrMailConversationId",PropertyValue ="090B191C3D1F4061BD36CB2F19242288"  },
                            new MetadataDto(){PropertyName ="dttedrMailReceived",PropertyValue ="04/08/2021 02:20"  },
                            new MetadataDto(){PropertyName ="dttedrMailIsReadReceiptRequested",PropertyValue ="False"  },
                            new MetadataDto(){PropertyName ="MailHeaders_MailFrom",PropertyValue ="bkonijet@deloitte.com"  },
                            new MetadataDto(){PropertyName ="dttedrMailConversationTopic",PropertyValue ="Solenis: Sigura Water and Platinum Equity Merger - Response Requested"  },
                            new MetadataDto(){PropertyName ="dttedrYear",PropertyValue ="2021"  },
                            new MetadataDto(){PropertyName ="dttedrMailImportance",PropertyValue ="Normal"  },
                            new MetadataDto(){PropertyName ="dttedrMailSent",PropertyValue ="04/08/2021 02:20"  },
                            new MetadataDto(){PropertyName ="dttedrMailHasAttachments",PropertyValue ="True"  },
                            new MetadataDto(){PropertyName ="dttedrDataClassification",PropertyValue ="Confidential"  },
                            new MetadataDto(){PropertyName ="dttedrRoutingInfo",PropertyValue =""  },

                        }
                    });
                }

                commitBatchRequest.ProjectTaskDetails = new ProjectTaskDto()
                {
                    TaskName = "Sample Task-Testing Task Details",
                    Description = "Sample task for testing creation of task new outlookmiddleware",
                    DueDate = System.DateTime.Now,
                    Priority = ProjectTaskPriority.High,
                    UserInvolvements = new List<ProjectTaskUserInvolvementDto>()
                    {
                        new ProjectTaskUserInvolvementDto()
                        {
                            PrincipalName = "kkaredla",
                            Type= ProjectTaskUserInvolvementType.AssignedBy,
                             UserId ="A51291C9-7165-469D-8844-8BE57E9F74F0"
                        },
                        new ProjectTaskUserInvolvementDto()
                        {
                            PrincipalName = "Bkonijeti",
                            Type= ProjectTaskUserInvolvementType.AssignedTo,
                             UserId ="184946A0-E943-4BDF-B5DB-1A5A04AC677B"
                        },new ProjectTaskUserInvolvementDto()
                        {
                            PrincipalName = "jhooyberghs",
                            Type= ProjectTaskUserInvolvementType.AssignedCc,
                             UserId ="85570FD1-3246-4693-AF49-E379398E5B37"
                        }

                    }
                };
                await HttpHelper.CommitBatch(commitBatchRequest);
            }
            _fileIds = new List<Guid>();
            _synchronizationContext.Send(_ =>
            {

                _committed = true;
                commitButton.Enabled = false;

            }, null);
        }
    }
}