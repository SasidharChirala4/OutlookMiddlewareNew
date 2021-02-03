using Edreams.OutlookMiddleware.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Edreams.OutlookMiddleware.Tests.Common
{
   public class TimeHelperTests
    {
        [Fact]
        public void IsGiveTimeWithinTimeSpan_LessThanStart()
        {
            // Arrange
            DateTime currentTime = new DateTime(2021, 02, 20, 16, 0, 0); // Time: 4:00 PM
            TimeSpan startTime = new TimeSpan(19, 0, 0); // Start Time: 7:00 PM
            TimeSpan stopTime = new TimeSpan(4, 0, 0); // Stop Time: 4:00 AM

            TimeHelper timeHelper = new TimeHelper();

            // Act
            bool isCurrentTimeWithinSpan = timeHelper.IsGivenTimeWithinTimeSpan(currentTime, startTime, stopTime);

            // Assert
            Assert.False(isCurrentTimeWithinSpan);
        }

        [Fact]
        public void IsGiveTimeWithinTimeSpan_MoreThanStop()
        {
            // Arrange
            DateTime currentTime = new DateTime(2021, 02, 20, 5, 0, 0); // Time: 5:00 AM
            TimeSpan startTime = new TimeSpan(19, 0, 0); // Start Time: 7:00 PM
            TimeSpan stopTime = new TimeSpan(4, 0, 0); // Stop Time: 4:00 AM

            TimeHelper timeHelper = new TimeHelper();

            // Act
            bool isCurrentTimeWithinSpan = timeHelper.IsGivenTimeWithinTimeSpan(currentTime, startTime, stopTime);

            // Assert
            Assert.False(isCurrentTimeWithinSpan);
        }

        [Fact]
        public void IsGiveTimeWithinTimeSpan_EqualToStart()
        {
            // Arrange
            DateTime currentTime = new DateTime(2021, 02, 20, 19, 0, 0); // Time: 7:00 PM
            TimeSpan startTime = new TimeSpan(19, 0, 0); // Start Time: 7:00 PM
            TimeSpan stopTime = new TimeSpan(4, 0, 0); // Stop Time: 4:00 AM

            TimeHelper timeHelper = new TimeHelper();

            // Act
            bool isCurrentTimeWithinSpan = timeHelper.IsGivenTimeWithinTimeSpan(currentTime, startTime, stopTime);

            // Assert
            Assert.True(isCurrentTimeWithinSpan);
        }       

        [Fact]
        public void IsGiveTimeWithinTimeSpan_Within()
        {
            //Arrange
            DateTime currentTime = new DateTime(2021, 02, 20, 20, 0, 0); //Time: 8:00 PM
            TimeSpan startTime = new TimeSpan(19, 0, 0); //Start Time: 7:00 PM
            TimeSpan stopTime = new TimeSpan(4, 0, 0); //Stop Time: 4:00 AM

            TimeHelper timeHelper = new TimeHelper();

            //Act
            bool isCurrentTimeWithinSpan = timeHelper.IsGivenTimeWithinTimeSpan(currentTime, startTime, stopTime);

            //Assert
            Assert.True(isCurrentTimeWithinSpan);

        }
    }
}
