using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.BusinessLogic.Factories;
using Edreams.OutlookMiddleware.BusinessLogic.Factories.Interfaces;
using Edreams.OutlookMiddleware.Model;
using Edreams.OutlookMiddleware.Model.Enums;
using FluentAssertions;
using Xunit;

namespace Edreams.OutlookMiddleware.Tests.Factories
{
    public class BatchFactoryTests
    {
        [Fact]
        public void BatchFactory_CreatePendingBatch_Should_Return_A_New_Batch()
        {
            // Arrange
            IBatchFactory batchFactory = new BatchFactory();

            // Act
            Batch result = batchFactory.CreatePendingBatch();

            // Assert 
            result.Should().NotBeNull();
            result.CreatedOn.Should().BeCloseTo(DateTime.UtcNow);
            result.CreatedBy.Should().Be("CREATED");
            result.Status.Should().Be(BatchStatus.Pending);
        }
    }
}