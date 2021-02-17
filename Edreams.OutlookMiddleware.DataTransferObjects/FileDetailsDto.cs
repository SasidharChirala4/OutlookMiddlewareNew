using System;

namespace Edreams.OutlookMiddleware.DataTransferObjects
{
    public class FileDetailsDto
    {
        public Guid Id { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
    }
}