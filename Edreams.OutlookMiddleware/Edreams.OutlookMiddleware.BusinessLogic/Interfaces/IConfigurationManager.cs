using System;
using System.Collections.Generic;
using System.Text;

namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    public interface IConfigurationManager
    {
        string GetValue(string configurationKey);
    }
}
