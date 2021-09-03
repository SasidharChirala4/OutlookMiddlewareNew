using System;
using System.Collections.Generic;
using System.Text;
using Edreams.OutlookMiddleware.Common.Helpers.Interfaces;

namespace Edreams.OutlookMiddleware.Common.Helpers
{
    public class TimeHelper : ITimeHelper
    {
        #region <| Construction |>
        /// <summary>
        /// Empty constructor for TimeHelper class
        /// </summary>
        public TimeHelper()
        {

        }
        #endregion

        #region <| Public Methods |>

        /// <summary>
        /// Method returning boolean value if given time is within a given timespan
        /// </summary>
        /// <param name="startTimeSpan"></param>
        /// <param name="stopTimeSpan"></param>
        /// <returns></returns>
        public bool IsGivenTimeWithinTimeSpan(DateTime currentTime, TimeSpan startTimeSpan, TimeSpan stopTimeSpan)
        {
            #region Validation
            if (startTimeSpan == stopTimeSpan)
            {
                return false;
            }
            #endregion
            DateTime start;
            DateTime stop;

            // For example Timespan: 21:00 - 06:00
            if (startTimeSpan > stopTimeSpan)
            {
                // currentTime - <22:00>
                if (currentTime.TimeOfDay >= stopTimeSpan)
                {
                    start = new DateTime(
                        currentTime.Year, currentTime.Month, currentTime.Day,
                        startTimeSpan.Hours, startTimeSpan.Minutes, startTimeSpan.Seconds);
                    stop = new DateTime(
                        currentTime.Year, currentTime.Month, currentTime.Day,
                        stopTimeSpan.Hours, stopTimeSpan.Minutes, stopTimeSpan.Seconds).AddDays(1);
                }
                // currentTime - <01:00>
                else
                {
                    start = new DateTime(
                        currentTime.Year, currentTime.Month, currentTime.Day,
                        startTimeSpan.Hours, startTimeSpan.Minutes, startTimeSpan.Seconds).AddDays(-1);

                    stop = new DateTime(
                        currentTime.Year, currentTime.Month, currentTime.Day,
                        stopTimeSpan.Hours, stopTimeSpan.Minutes, stopTimeSpan.Seconds);
                }
            }
            // 06:00 - 21:00
            else
            {
                start = new DateTime(
                    currentTime.Year, currentTime.Month, currentTime.Day,
                    startTimeSpan.Hours, startTimeSpan.Minutes, startTimeSpan.Seconds);

                stop = new DateTime(
                    currentTime.Year, currentTime.Month, currentTime.Day,
                    stopTimeSpan.Hours, stopTimeSpan.Minutes, stopTimeSpan.Seconds);
            }

            return currentTime >= start && currentTime <= stop;
        }
        #endregion
    }

}

