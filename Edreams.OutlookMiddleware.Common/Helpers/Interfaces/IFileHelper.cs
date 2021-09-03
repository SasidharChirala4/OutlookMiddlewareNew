using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.Common.Helpers.Interfaces
{
    public interface IFileHelper
    {
        /// <summary>
        /// Read the file data
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        Task<byte[]> LoadFileInMemory(string filePath);

        /// <summary>
        /// Deletes the file 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        Task DeleteFile(string filePath);

        /// <summary>
        /// Deletes the list of files 
        /// </summary>
        /// <param name="filesPath"></param>
        /// <returns></returns>
        Task DeleteFile(List<string> filesPath);
    }
}