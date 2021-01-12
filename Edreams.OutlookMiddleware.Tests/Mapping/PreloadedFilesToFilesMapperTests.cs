﻿using System;
using System.Collections.Generic;
using System.Linq;
using Edreams.OutlookMiddleware.Mapping.Custom;
using Edreams.OutlookMiddleware.Mapping.Custom.Interfaces;
using Edreams.OutlookMiddleware.Model;
using Edreams.OutlookMiddleware.Model.Enums;
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

            #endregion

            #region [ ACT ]

            IList<File> files = mapper.Map(batch, preloadedFiles);

            #endregion

            #region [ ASSERT ]

            files.Count.Should().Be(preloadedFiles.Count);
            files.Select(x => x.Email).Select(x => x.Batch).Should().AllBeEquivalentTo(batch);
            files.Select(x => x.Email).Distinct().Count().Should().Be(2);

            #endregion
        }

        #endregion
    }
}