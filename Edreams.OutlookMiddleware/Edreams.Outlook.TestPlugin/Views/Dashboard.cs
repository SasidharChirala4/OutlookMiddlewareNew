using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Edreams.Outlook.TestPlugin.Helpers;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Microsoft.Office.Interop.Outlook;

namespace Edreams.Outlook.TestPlugin.Views
{
    public partial class Dashboard : UserControl
    {
        private readonly SynchronizationContext _synchronizationContext;
        private List<MailItem> _selection;
        private Guid? _batchId;
        private bool _preloaded;
        private bool _committed;

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

            preloadButton.Enabled = false;

            int currentSelected = 0;

            foreach (MailItem mail in _selection)
            {
                try
                {
                    currentSelected++;

                    string ewsId = await ExchangeHelper.ConvertEntryIdToEwsId(mail.EntryID);

                    var ewsEmail = await ExchangeHelper.DownloadEmail(ewsId);

                    var createMailRequest = new CreateMailRequest
                    {
                        BatchId = batchId,
                        CorrelationId = correlationId,
                        MailEntryId = mail.EntryID,
                        MailEwsId = ewsId,
                        MailSubject = ewsEmail.Subject
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
                    }

                    foreach (var attachment in createMailResponse.Attachments)
                    {
                        var binary = ewsEmail.Attachments.Single(x => x.Id == attachment.AttachmentId);
                        using (var memoryStream = new MemoryStream(binary.Data))
                        {
                            await HttpHelper.UploadAsync(memoryStream, binary.Name, attachment.FileId);
                        }
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

        private async void commitButton_Click(object sender, System.EventArgs e)
        {
            if (_preloaded && _batchId.HasValue)
            {
                await HttpHelper.CommitBatch(_batchId.Value);
            }

            _synchronizationContext.Send(_ =>
            {

                _committed = true;
                commitButton.Enabled = false;

            }, null);
        }
    }
}