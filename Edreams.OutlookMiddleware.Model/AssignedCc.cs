using Edreams.Common.DataAccess.Model;

namespace Edreams.OutlookMiddleware.Model
{
    public class AssignedCc : ModelBase
    {
        public EmailAddress EmailAddress { get; set; }
        public Task Task { get; set; }
    }
}
