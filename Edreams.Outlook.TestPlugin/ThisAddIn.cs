using System;
using System.Collections.Generic;
using Edreams.Outlook.TestPlugin.Helpers;
using Edreams.Outlook.TestPlugin.Views;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.Enums;
using Microsoft.Office.Interop.Outlook;
using Microsoft.Office.Tools;
using Office = Microsoft.Office.Core;

namespace Edreams.Outlook.TestPlugin
{
    public partial class ThisAddIn
    {
        public Explorer Explorer { get; private set; }

        private Dashboard _dashboard;
        private CustomTaskPane _customTaskPane;
        public const string SchemaInternetMessageId = "http://schemas.microsoft.com/mapi/proptag/0x1035001F";
        private void ThisAddIn_Startup(object sender, EventArgs e)
        {
            Explorer = Application.ActiveExplorer();
            Explorer.SelectionChange += Explorer_SelectionChange;

            _dashboard = new Dashboard();
            _customTaskPane = CustomTaskPanes.Add(_dashboard, "Outlook Middleware Test");
            _customTaskPane.Width = 500;
            _customTaskPane.Visible = true;
            Application.ItemSend += Application_ItemSendAsync;

        }

        private void Application_ItemSendAsync(object Item, ref bool Cancel)
        {

            MailItem mailItem = Item as MailItem;
            if (mailItem != null)
            {
                if (mailItem.EntryID == null) { mailItem.Save(); }
                Guid edreamsReferenceId = Guid.NewGuid();
                string middlewareMailBox = "kkaredla@deloitte.com";
                if (!string.IsNullOrEmpty(middlewareMailBox))
                {
                    Recipient recipient = mailItem.Recipients.Add(middlewareMailBox);
                    recipient.Type = (int)OlMailRecipientType.olBCC;
                    mailItem.Recipients.ResolveAll();

                    UserProperty userProperty = mailItem.UserProperties.Add("EdreamsReferenceId", OlUserPropertyType.olText);
                    userProperty.Value = $"{edreamsReferenceId}";
                }
                mailItem.Save();

                #region PreloadFiles
                Guid batchId = Guid.NewGuid();
                Guid correlationId = Guid.NewGuid();
                int currentSelected = 0;
                try
                {
                    currentSelected++;
                    string ewsId = ExchangeHelper.ConvertEntryIdToEwsId(mailItem.EntryID).Result;
                    //var ewsEmail = ExchangeHelper.DownloadEmail(ewsId).Result;
                    //var internetMessageId = mailItem.PropertyAccessor.GetProperty(SchemaInternetMessageId);
                    var createMailRequest = new CreateMailRequest
                    {
                        BatchId = batchId,
                        CorrelationId = correlationId,
                        MailEntryId = mailItem.EntryID,
                        MailEwsId = ewsId,
                        EdreamsReferenceId = edreamsReferenceId,
                        EmailKind = OutlookMiddleware.Enums.EmailKind.Sent
                    };

                    for (int i = 0; i < mailItem.Attachments.Count; i++)
                    {
                        createMailRequest.Attachments.Add(
                        new OutlookMiddleware.DataTransferObjects.Api.Specific.Attachment
                        {
                            Id = string.Empty,
                            Name = string.Empty
                        });
                    }
                    var createMailResponse = HttpHelper.CreateMail(createMailRequest).Result;
                    
                }
                catch
                {
                    // Nothing we can do...
                }
                #endregion

                #region CommitBatch
                //Calling CommitBatch Endpoint
                CommitBatchRequest commitBatchRequest = new CommitBatchRequest()
                {
                    BatchId = batchId,
                    UploadOption = EmailUploadOptions.Emails,
                    //EmailRecipients = new List<EmailRecipientDto>() { new EmailRecipientDto() { EmailId=new Guid("E400B31C-BC09-4F4F-B16F-546181285D67") ,Type=EmailRecipientType.Contact,Recipient="kkaredla@deloitte.com" },
                    // new EmailRecipientDto() { EmailId=new Guid("7F6856BF-6490-4D60-A52B-FE1301C894CD") ,Type=EmailRecipientType.Contact,Recipient="bkonijeti@deloitte.com" }}
                };
                HttpHelper.CommitBatch(commitBatchRequest).Wait();
                #endregion
            }
        }

        private void ThisAddIn_Shutdown(object sender, EventArgs e)
        {
            // Note: Outlook no longer raises this event. If you have code that 
            //    must run when Outlook shuts down, see https://go.microsoft.com/fwlink/?LinkId=506785
        }

        private async void Explorer_SelectionChange()
        {
            List<MailItem> selection = new List<MailItem>();

            foreach (object selected in Explorer.Selection)
            {
                if (selected is MailItem mail)
                {
                    selection.Add(mail);
                }
            }

            await _dashboard.UpdateSelection(selection);
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            Startup += ThisAddIn_Startup;
            Shutdown += ThisAddIn_Shutdown;
        }

        #endregion

        protected override Office.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            return new EdreamsRibbon();
        }
    }
}