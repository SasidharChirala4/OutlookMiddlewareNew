using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edreams.OutlookMiddleware.Mapping.Custom.Interfaces
{
    public interface IProjectTaskDetailsDtoToProjectTaskMapper
    {
        ProjectTask Map(ProjectTaskDetailsDto projectTaskDetails,Email email);
    }
}
