using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Edreams.Outlook.TestPlugin.Model;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Edreams.Outlook.TestPlugin.Helpers
{
    public static class ExchangeHelper
    {
        private static string _token = null;

        public static async Task<string> ConvertEntryIdToEwsId(string entryId)
        {
            ExchangeService exchangeService = await CreateExchangeService();
            return await exchangeService.ConvertId(entryId);
        }

        public static async Task<EwsEmail> DownloadEmail(string ewsId)
        {
            ExchangeService exchangeService = await CreateExchangeService();

            var propertySet = new PropertySet(
                BasePropertySet.FirstClassProperties, ItemSchema.MimeContent, ItemSchema.Attachments);
            var result = await exchangeService.BindToItems(new[] { new ItemId(ewsId), ewsId }, propertySet);

            EwsEmail email = new EwsEmail
            {
                EwsId = ewsId,
                Subject = result[0].Item.Subject,
                Data = result[0].Item.MimeContent.Content
            };

            foreach (var attachment in result[0].Item.Attachments)
            {
                if (attachment is FileAttachment fileAttachment)
                {
                    await fileAttachment.Load();
                    email.Attachments.Add(new EwsAttachment
                    {
                        Id = fileAttachment.Id,
                        Name = fileAttachment.Name,
                        Data = fileAttachment.Content
                    });
                }
            }

            return email;
        }

        private static async Task<ExchangeService> CreateExchangeService()
        {
            if (_token == null)
            {
                var ctx = new AuthenticationContext("https://login.microsoftonline.com/deloitte.onmicrosoft.com/");
                var result = await ctx.AcquireTokenAsync(
                    "https://outlook.office365.com",
                    "2fae4fbb-2f89-4db4-925b-5d117fc51cb2",
                    new Uri("http://deloitte.com/edreamsofficeaddin-dev"),
                    new PlatformParameters(PromptBehavior.Auto));
                _token = result.AccessToken;
            }

            return new ExchangeService
            {
                Credentials = new OAuthCredentials(_token),
                Url = new Uri("https://outlook.office365.com/ews/exchange.asmx")
            };
        }

        private static async Task<string> ConvertId(this ExchangeService exchangeService, string entryId)
        {
            AlternateId alternateId = new AlternateId
            {
                Format = IdFormat.HexEntryId,
                Mailbox = "jhooyberghs@deloitte.com",
                UniqueId = entryId
            };

            AlternateIdBase alternateIdBase = await exchangeService.ConvertId(alternateId, IdFormat.EwsId);
            AlternateId alternateIdResult = (AlternateId)alternateIdBase;
            return alternateIdResult.UniqueId;
        }
    }
}