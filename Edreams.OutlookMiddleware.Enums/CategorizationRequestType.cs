namespace Edreams.OutlookMiddleware.Enums
{
    /// <summary>
    /// Categorization Request Type Enum
    /// </summary>
    public enum CategorizationRequestType
    {
        EmailUploaded = 10,
        AttachmentUploaded = 20,
        EmailAndAttachmentUploaded = 30,
        EmailUploadFailed = 40,
        AtachmentUploadFailed = 50,
        EmailAndAttachmentUploadFailed = 60
        
    }
}
