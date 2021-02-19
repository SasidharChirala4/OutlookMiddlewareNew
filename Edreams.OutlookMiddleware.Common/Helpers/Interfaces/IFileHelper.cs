using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.Common.Helpers.Interfaces
{
    public interface IFileHelper
    {
        Task<byte[]> LoadFileInMemory(string filePath);
    }
}