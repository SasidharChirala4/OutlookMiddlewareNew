namespace Edreams.OutlookMiddleware.Common.Configuration.Interfaces
{
    public interface IEdreamsConfiguration
    {
        public string StoragePath { get; set; }
        public string EdreamsExtensibilityUrl { get; set; }
        public string EdreamsTokenKey { get; set; }
        public string EdreamsTokenValue { get; set; }
        public string MaxNumberPendingCategories { get; set; }
    }
}