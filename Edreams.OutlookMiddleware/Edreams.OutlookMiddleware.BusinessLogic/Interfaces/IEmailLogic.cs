﻿using System.Threading.Tasks;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;

namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    public interface IEmailLogic
    {
        Task<CreateMailResponse> CreateMail(CreateMailRequest request);
    }
}