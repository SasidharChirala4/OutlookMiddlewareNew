using Edreams.OutlookMiddleware.Enums;
using System;

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