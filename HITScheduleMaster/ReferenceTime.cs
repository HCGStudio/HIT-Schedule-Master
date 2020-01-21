using System;
using System.Collections.Generic;

namespace HCGStudio.HITScheduleMaster
{
    internal static class ReferenceTime
    {
        public static Dictionary<string, DateTime> BaseTime => new Dictionary<string, DateTime>
        {
            {"2019-20201", new DateTime(2019, 09, 02)}, //2019秋
            {"2019-20202", new DateTime(2020, 02, 24)}, //2020春
            {"2019-20203", new DateTime(2020, 06, 29)} //2020夏
        };

        public static TimeSpan ClassLength => new TimeSpan(01, 45, 00);

        public static TimeSpan[] StartTimes => new[]
        {
            new TimeSpan(),
            new TimeSpan(08, 00, 00),
            new TimeSpan(10, 00, 00),
            new TimeSpan(13, 45, 00),
            new TimeSpan(15, 45, 00),
            new TimeSpan(18, 30, 00)
        };
    }
}