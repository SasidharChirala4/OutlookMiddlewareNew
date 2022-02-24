using System;
using System.Collections.Generic;
using System.Text;

namespace Edreams.OutlookMiddleware.Common.Constants
{
    public static class Constants
    {
        public const string EdrMailCc = "MailHeaders_MailCc";
        public const string EdrMailTo = "MailHeaders_MailTo";
        public const string EdrMailFrom = "MailHeaders_MailFrom";
        public const string EdrMailSent = "MailHeaders_MailSent";
        public const string EdrMailSubject = "MailHeaders_MailSubject";

        public const string MailHeaderTemplatePara = "<div style = 'border:none;padding:3.0pt 0in 0in 0in'><p class=MsoNormal>{0}</p></div><br>";
        public const string MailHeaderTemplateRow = @"<b>{0}: </b>{1}<br>";
        public const string MailFrom = "From";
        public const string MailTo = "To";
        public const string MailCc = "Cc";
        public const string MailSent = "Sent";
        public const string MailSubject = "Subject";

        public const string MailHeaderSendFormat = "dddd, dd MMMM, yyyy HH:mm";
        public const string EdrDateTimeFormat = "dd/MM/yyyy HH:mm";
    }
}
