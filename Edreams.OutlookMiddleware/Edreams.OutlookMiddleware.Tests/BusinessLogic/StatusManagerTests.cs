using Edreams.OutlookMiddleware.BusinessLogic;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace Edreams.OutlookMiddleware.Tests.BusinessLogic
{
    public class StatusManagerTests
    {
        [Fact]
        public async Task StatusManager_GetStatus_Should_Return_An_Instance_Of_GetStatusResponse()
        {
            // Arrange
            StatusManager statusManager = new StatusManager();

            // Act
            GetStatusResponse response = await statusManager.GetStatus();

            // Assert
            response.Should().NotBeNull();
            response.CorrelationId.Should().NotBeEmpty();
        }
    }
}