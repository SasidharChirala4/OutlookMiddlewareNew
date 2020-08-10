using System.Collections.Generic;

namespace Edreams.Outlook.TestPlugin.Model
{
    public class EwsEmail
    {
        public EwsEmail()
        {
            Attachments = new List<EwsAttachment>();
        }

        public string Subject { get; set; }
        public string EwsId { get; set; }
        public byte[] Data { get; set; }

        public IList<EwsAttachment> Attachments { get; }
    }
}
