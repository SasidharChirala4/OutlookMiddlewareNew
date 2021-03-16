using System.Collections.Generic;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.Mapping.Custom.Interfaces;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.Mapping.Custom
{
    public class EmailsToEmailDetailsMapper : IEmailsToEmailDetailsMapper
    {
        public IList<EmailDetailsDto> Map(IList<Email> emails)
        {
            List<EmailDetailsDto> listOfEmailDetails = new List<EmailDetailsDto>(emails.Count);

            foreach (Email email in emails)
            {
                List<FileDetailsDto> listOfFileDetails = new List<FileDetailsDto>(email.Files.Count);

                foreach (File file in email.Files)
                {

                    FileDetailsDto fileDetails = new FileDetailsDto
                    {
                        Id = file.Id,
                        Name = file.FileName,
                        Path = file.TempPath,
                        Kind = file.Kind
                    };

                    listOfFileDetails.Add(fileDetails);
                }

                EmailDetailsDto emailDetails = new EmailDetailsDto
                {
                    Id = email.Id,
                    Files = listOfFileDetails,
                    UploadOption = email.UploadOption
                };

                listOfEmailDetails.Add(emailDetails);
            }

            return listOfEmailDetails;
        }
    }
}