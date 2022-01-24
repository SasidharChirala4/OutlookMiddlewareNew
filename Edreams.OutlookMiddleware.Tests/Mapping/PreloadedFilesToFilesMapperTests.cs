using System;
using System.Collections.Generic;
using System.Linq;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Mapping.Custom;
using Edreams.OutlookMiddleware.Mapping.Custom.Interfaces;
using Edreams.OutlookMiddleware.Model;
using FluentAssertions;
using Xunit;

namespace Edreams.OutlookMiddleware.Tests.Mapping
{
    public class PreloadedFilesToFilesMapperTests
    {
        #region <| CommitBatch |>

        [Fact]
        public void PreloadedFilesToFilesMapper_Map_Should_Work()
        {
            #region [ ARRANGE ]

            IPreloadedFilesToFilesMapper mapper = new PreloadedFilesToFilesMapper();

            Batch batch = new Batch
            {
                Id = Guid.NewGuid(),
                Status = BatchStatus.Pending
            };

            Guid email1Id = Guid.NewGuid();
            Guid email2Id = Guid.NewGuid();
            IList<FilePreload> preloadedFiles = new List<FilePreload>
            {
                new FilePreload
                {
                    Id = Guid.NewGuid(),
                    BatchId = batch.Id,
                    EmailId = email1Id
                },
                new FilePreload
                {
                    Id = Guid.NewGuid(),
                    BatchId = batch.Id,
                    EmailId = email1Id
                },
                new FilePreload
                {
                    Id = Guid.NewGuid(),
                    BatchId = batch.Id,
                    EmailId = email2Id
                }
            };

            CommitBatchRequest request = new CommitBatchRequest
            {
                UploadOption = EmailUploadOptions.EmailsAndAttachments,
                EmailRecipients = new List<EmailRecipientDto>(),
                Files = preloadedFiles.Select(x => new FileDetailsDto { Id = x.Id, ShouldUpload = true, Metadata = new List<MetadataDto>() }).ToList()
            };

            #endregion

            #region [ ACT ]

            IList<File> files = mapper.Map(batch, preloadedFiles, request);

            #endregion

            #region [ ASSERT ]

            files.Count.Should().Be(preloadedFiles.Count);
            files.Select(x => x.Email).Select(x => x.Batch).Should().AllBeEquivalentTo(batch);
            files.Select(x => x.Email).Distinct().Count().Should().Be(2);

            #endregion
        }

        [Fact]
        public void PreloadedFilesMapper_Map_Should_Return_Null()
        {
            #region [ ARRANGE ]

            IPreloadedFilesToFilesMapper mapper = new PreloadedFilesToFilesMapper();

            Batch batch = new Batch
            {
                Id = Guid.NewGuid(),
                Status = BatchStatus.Pending
            };
            IList<FilePreload> preloadedFiles = new List<FilePreload> { };

            CommitBatchRequest request = new CommitBatchRequest
            {
                EmailRecipients = new List<EmailRecipientDto>(),
                Files = new List<FileDetailsDto>(),
            };

            #endregion

            #region [ ACT ]

            IList<File> files = mapper.Map(batch, preloadedFiles, request);

            #endregion

            #region [ASSERT]

            files.Should().HaveCount(0);

            #endregion
        }

        #endregion
    }
}