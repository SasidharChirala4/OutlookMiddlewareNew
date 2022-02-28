using System;
using Edreams.Common.DataAccess.Model;
using Edreams.Common.DataAccess.Model.Interfaces;
using Edreams.OutlookMiddleware.Enums;

namespace Edreams.OutlookMiddleware.Model
{
    public class Batch : ModelBase, ILongSysId
    {
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public BatchStatus Status { get; set; }
        public string UploadLocationSite { get; set; }
        public EmailUploadOptions UploadOption { get; set; }        
        public string UploadLocationFolder { get; set; }
        public string VersionComment { get; set; }
        public bool DeclareAsRecord { get; set; }
        public string PrincipalName { get; set; }


    }
}