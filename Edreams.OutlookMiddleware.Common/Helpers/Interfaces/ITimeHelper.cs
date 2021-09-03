using System;
using System.Collections.Generic;
using System.Text;

namespace Edreams.OutlookMiddleware.Common.Helpers.Interfaces
{
    public interface ITimeHelper
    {
        bool IsGivenTimeWithinTimeSpan(DateTime currentTime, TimeSpan startTimeSpan, TimeSpan stopTimeSpan);
    }
}
