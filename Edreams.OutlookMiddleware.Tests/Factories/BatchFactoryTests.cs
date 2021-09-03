using System;
using Edreams.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Factories;
using Edreams.OutlookMiddleware.BusinessLogic.Factories.Interfaces;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Model;
using FluentAssertions;
using Moq;
using Xunit;

namespace Edreams.OutlookMiddleware.Tests.Factories
{
    public class BatchFactoryTests
    {
        [Fact]
        public void BatchFactory_CreatePendingBatch_Should_Return_A_New_Batch()
        {
            // Arrange
            var securityContextMock = new Mock<ISecurityContext>();
            IBatchFactory batchFactory = new BatchFactory(securityContextMock.Object);

            // Mock
            securityContextMock.Setup(x => x.PrincipalName).Returns("Mr. Skittles");

            // Act
            Batch result = batchFactory.CreatePendingBatch();

            // Assert 
            result.Should().NotBeNull();
            result.CreatedOn.Should().BeCloseTo(DateTime.UtcNow, precision: 100);
            result.CreatedBy.Should().Be("Mr. Skittles");
            result.Status.Should().Be(BatchStatus.Pending);
        }
    }
}