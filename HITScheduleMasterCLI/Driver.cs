using System;
using System.Collections.Generic;
using System.Text;
using MobileSuit;
using ScheduleTranslator;
using MobileSuit.IO;

namespace HITScheduleMasterCLI
{
    public class Driver : IIoInteractive
    {
        public void A()
        {
            for (; ; )
                ShowScheduleEntry(new ScheduleEntry(DayOfWeek.Monday, CourseTime.C12, Io?.ReadLine()));
        }
        private void ShowScheduleEntry(ScheduleEntry entry)
        {
            var outList = new List<(string, ConsoleColor?)>
            {
                (entry.CourseName.PadRight(24,' '), null),
                (entry.DayOfWeekName, null),
                ("\t", null),
                (entry.CourseTimeName, null),
                ("\t", null),
                (entry.StartTime.ToString(), null),
                ("\t", null),
                (entry.Teacher, null),
                ("\t", null),
                (entry.Location==""?"\t":entry.Location, null),
                ("\t", null),
                (entry.IsLongCourse ? "2" : "3", null),
                ("\t", null)
            };
            for (var i = entry.Week >> 1; i != 0; i >>= 1)
            {

                if ((i & 1) == 1)
                {
                    outList.Add((" 1  ", ConsoleColor.Green));
                }
                else
                {
                    outList.Add((" 0  ", ConsoleColor.Red));
                }
            }

            Io?.WriteLine(outList);
        }
        private IoInterface Io { get; set; }

        public void SetIo(IoInterface io)
        {
            Io = io;
        }
    }
}
