using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.Common.Exceptions.Interfaces;
using Edreams.OutlookMiddleware.Common.Helpers.Interfaces;

namespace Edreams.OutlookMiddleware.Common.Helpers
{
    public class FileHelper : IFileHelper
    {
        private readonly IExceptionFactory _exceptionFactory;

        public FileHelper(IExceptionFactory exceptionFactory)
        {
            _exceptionFactory = exceptionFactory;
        }

        /// <summary>
        /// Reads the file data
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public Task<byte[]> LoadFileInMemory(string filePath)
        {
            try
            {
                return File.ReadAllBytesAsync(filePath);
            }
            catch (ArgumentNullException)
            {
                // The specified path, file name, or both exceed the system-defined maximum length.

                // TODO: should not retry.
                throw;
            }
            catch (ArgumentException)
            {
                // Path is a zero-length string, contains only white space, or contains
                // one or more invalid characters as defined by InvalidPathChars.

                // TODO: should not retry.
                throw;
            }
            catch (DirectoryNotFoundException)
            {
                // The specified path is invalid (for example, it is on an unmapped drive).

                // TODO: should not retry.
                throw;
            }
            catch (UnauthorizedAccessException)
            {
                // This operation is not supported on the current platform.
                // The path specified a directory.
                // The caller does not have the required permission.

                // TODO: should not retry.
                throw;
            }
            catch (FileNotFoundException)
            {
                // The file specified in path was not found.

                // TODO: should not retry.
                throw;
            }
            catch (IOException)
            {
                // An I/O error occurred while opening the file.

                // TODO: should retry.
                throw;
            }
            catch (NotSupportedException)
            {
                // The path is in an invalid format.

                // TODO: should not retry.
                throw;
            }
            catch (SecurityException)
            {
                // The caller does not have the required permission.

                // TODO: should not retry.
                throw;
            }
        }

        /// <summary>
        /// Deletes the file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public Task DeleteFile(string filePath)
        {
            try
            {
                return Task.Run(() =>
                {
                    File.Delete(filePath);
                });
            }
            catch (ArgumentNullException)
            {
                // The specified path, file name, or both exceed the system-defined maximum length.

                // TODO: should not retry.
                throw;
            }
            catch (ArgumentException)
            {
                // Path is a zero-length string, contains only white space, or contains
                // one or more invalid characters as defined by InvalidPathChars.

                // TODO: should not retry.
                throw;
            }
            catch (DirectoryNotFoundException)
            {
                // The specified path is invalid (for example, it is on an unmapped drive).

                // TODO: should not retry.
                throw;
            }
            catch (UnauthorizedAccessException)
            {
                // This operation is not supported on the current platform.
                // The path specified a directory.
                // The caller does not have the required permission.

                // TODO: should not retry.
                throw;
            }
            catch (FileNotFoundException)
            {
                // The file specified in path was not found.

                // TODO: should not retry.
                throw;
            }
            catch (IOException)
            {
                // An I/O error occurred while opening the file.

                // TODO: should retry.
                throw;
            }
            catch (NotSupportedException)
            {
                // The path is in an invalid format.

                // TODO: should not retry.
                throw;
            }
            catch (SecurityException)
            {
                // The caller does not have the required permission.

                // TODO: should not retry.
                throw;
            }
        }

        /// <summary>
        /// Deletes list of files
        /// </summary>
        /// <param name="filesPath"></param>
        /// <returns></returns>
        public async Task DeleteFile(List<string> filesPath)
        {
            foreach (string filePath in filesPath)
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    await DeleteFile(filePath);
                }
            }
        }
    }
}