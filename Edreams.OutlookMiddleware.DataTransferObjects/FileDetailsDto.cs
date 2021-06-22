using System;
using Edreams.OutlookMiddleware.Enums;

namespace Edreams.OutlookMiddleware.DataTransferObjects
{
    public class FileDetailsDto
    {
        public Guid Id { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public FileKind Kind { get; set; }
        public string EmailSubject { get; set; }
    }
}