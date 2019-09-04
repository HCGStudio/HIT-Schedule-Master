using System;
using System.Collections.Generic;

namespace HCGStudio.HITScheduleMaster
{
    public class Course
    {
        public string Name { get; set; }
        public string Teacher { get; set; }
        public string Location { get; set; }
        public bool? IsEnableAlarm { get; set; }
        public int NotifyTime { get; set; }
        public List<RelativeTime> Times { get; set; }

        public class RelativeTime
        {
            public int Week { get; set; }
            public int WeekDay { get; set; }
            public int Sequent { get; set; }

            public DateTime ToDateTime(string semester)
            {
                return ReferenceTime.BaseTime[semester].AddDays((Week - 1) * 7 + WeekDay - 1) +
                       ReferenceTime.StartTimes[Sequent];
            }
        }
    }
}