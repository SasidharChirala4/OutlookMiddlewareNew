using System;
using System.Threading.Tasks;
using Edreams.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using FluentAssertions;
using Moq;
using Xunit;

namespace Edreams.OutlookMiddleware.Tests.BusinessLogic
{
    public class StatusManagerTests
    {
        [Fact]
        public async Task StatusManager_GetStatus_Should_Return_An_Instance_Of_GetStatusResponse()
        {
            // Arrange
            Mock<ISecurityContext> securityContextMock = new Mock<ISecurityContext>();
            StatusManager statusManager = new StatusManager(securityContextMock.Object);
            Guid correlationId = Guid.NewGuid();

            // Mock
            securityContextMock.Setup(x => x.CorrelationId).Returns(correlationId);

            // Act
            GetStatusResponse response = await statusManager.GetStatus();

            // Assert
            response.Should().NotBeNull();
            response.CorrelationId.Should().Be(correlationId);
        }
    }
}