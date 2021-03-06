using System;
using System.Collections.Generic;
using Edreams.OutlookMiddleware.Enums;

namespace Edreams.OutlookMiddleware.DataTransferObjects
{
    /// <summary>
    /// Contract that represents File Details
    /// </summary>
    public class FileDetailsDto
    {
        /// <summary>
        /// File id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// File id
        /// </summary>
        public Guid EmailId { get; set; }

        /// <summary>
        /// File path
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// File original name
        /// </summary>
        public string OriginalName { get; set; }

        /// <summary>
        /// File new name
        /// </summary>
        public string NewName { get; set; }

        /// <summary>
        /// File extension
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// Should this file upload or not
        /// </summary>
        public bool ShouldUpload { get; set; }

        /// <summary>
        /// File kind
        /// </summary>
        public FileKind Kind { get; set; }

        /// <summary>
        /// Email subject
        /// </summary>
        public string EmailSubject { get; set; }

        /// <summary>
        /// List of Metadata
        /// </summary>
        public List<MetadataDto> Metadata { get; set; }
    }
}