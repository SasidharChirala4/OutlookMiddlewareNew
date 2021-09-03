using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.Model;
using System;

namespace Edreams.OutlookMiddleware.Mapping.Custom.Interfaces
{
    public interface IProjectTaskDetailsDtoToProjectTaskMapper
    {
        ProjectTask Map(ProjectTaskDto projectTaskDetails, Email email);
    }
}
