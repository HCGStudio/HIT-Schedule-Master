using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ical.Net.Serialization;
using MobileSuit;
using ScheduleTranslator;
using MobileSuit.IO;
using static Newtonsoft.Json.JsonConvert;

namespace HITScheduleMasterCLI
{
    public class Driver : IIoInteractive
    {
        private Schedule Schedule { get; set; }
        
        public void Add()
        {
            var name = Io?.ReadLine("输入课程名称");
            if (!int.TryParse(Io?.ReadLine("输入星期(0-6,0表示周日)"), out var week)
            ||week<0||week>6) goto WrongInput;
            var teacher = Io?.ReadLine("输入教师");
            var location = Io?.ReadLine("输入地点");
            if (!TimeSpan.TryParse(Io?.ReadLine("输入开始时间(hh:mm:ss)"), out var startTime))
                goto WrongInput;
            if (!TimeSpan.TryParse(Io?.ReadLine("输入持续时间(hh:mm:ss)"), out var length))
                goto WrongInput;
            
            var weekExpression = Io?.ReadLine("输入周数(周数/起始-截至[单/双/(无)])");
            Schedule.Entries.Add(new ScheduleEntry((DayOfWeek) week, default,
                    $"{name}<>{teacher}[{weekExpression}]@{location}")
                {StartTime = startTime, Length = length});
            return;
        WrongInput:
            Io?.WriteLine("非法输入。", IoInterface.OutputType.Error);

        }
        public void ImpCse(string path)
        {
            if (!File.Exists(path))
            {
                Io?.WriteLine("未找到文件。", IoInterface.OutputType.Error);
                return;
            }
            Schedule.Entries.Add(DeserializeObject<ScheduleEntry>(File.ReadAllText(path)));
        }
        public void ExpCse(string id, string path = "")
        {
            if (Schedule is null) return;

            if (!int.TryParse(id, out var index) || index < 0 || index >= Schedule.Entries.Count) return;
            if (path == "")
                path = Io?.ReadLine("输入保存文件位置");
            try
            {
                File.WriteAllText(path, SerializeObject(Schedule.Entries[index]));
            }
            catch
            {
                Io?.WriteLine("写入错误。", IoInterface.OutputType.Error);
                Environment.Exit(0);
            }
        }
        public void Remove(string id)
        {
            if (Schedule is null) return;
            if (int.TryParse(id, out var index) && index >= 0 && index < Schedule.Entries.Count)
            {
                Schedule.Entries.RemoveAt(index);
            }
        }
        public void Edit(string id)
        {
            if (Schedule is null) return;
            if (int.TryParse(id, out var index) && index >= 0 && index < Schedule.Entries.Count)
            {
                var course = Schedule.Entries[index];
                course.CourseName = Io?.ReadLine($"输入课程名称={course.CourseName}",course.CourseName,
                    null);
                if (!int.TryParse(Io?.ReadLine($"输入星期(0-6,0表示周日)={((int)course.DayOfWeek)}",
                            ((int)course.DayOfWeek).ToString(),null)
                    , out var week)
                    || week < 0 || week > 6) goto WrongInput;
                course.DayOfWeek = (DayOfWeek) week;
                course.Teacher = Io?.ReadLine($"输入教师={course.Teacher}", course.Teacher,null);
                course.Location = Io?.ReadLine($"输入地点={course.Location}", course.Location,null);
                if (!TimeSpan.TryParse(Io?.ReadLine($"输入开始时间(hh:mm:ss)={course.StartTime}",
                    course.StartTime.ToString(),null), out var startTime))
                    goto WrongInput;
                course.StartTime = startTime;
                if (!TimeSpan.TryParse(Io?.ReadLine($"输入持续时间(hh:mm:ss)={course.Length}",
                    course.Length.ToString(), null), out var length))
                    goto WrongInput;
                course.Length = length;
                var weekExpression = Io?.ReadLine(
                    $"输入周数(周数/起始-截至[单/双/(无)]，空格隔开)={course.WeekExpression}",course.WeekExpression,null);
                if (weekExpression != course.WeekExpression)
                {
                    course.ChangeWeek(weekExpression);
                }
                return;

            }
            WrongInput:
            Io?.WriteLine("非法输入。", IoInterface.OutputType.Error);
        }
        public void Export(string path = "")
        {
            ScheduleCheck();
            if (Schedule is null) return;
            if (path == "")
                path = Io?.ReadLine("输入保存文件位置");
            try
            {
                var calendar = Schedule.GetCalendar();
                //calendar.Name = Io?.ReadLine($"输入课表名称(默认:{calendar.Name})", calendar.Name, null);
                File.WriteAllText(path,
                    new CalendarSerializer().SerializeToString(calendar),
                    new UTF8Encoding(false));
            }
            catch
            {
                Io?.WriteLine("写入错误。", IoInterface.OutputType.Error);
                Environment.Exit(0);
            }
        }
        public void Show()
        {
            ScheduleCheck();
            if (Schedule is null) return;
            var maxWeek = (from e in Schedule.Entries select e.MaxWeek).Max();
            var outList = new List<(string, ConsoleColor?)>
            {
                ("编号",null),
                ("\t", null),
                ("课程名".PadRight(22,' '), null),
                ("\t", null),
                ("星期", null),
                ("\t\t", null),
                ("起始时间", null),
                ("\t", null),
                ("课程长度",null),
                ("\t", null),
                ("授课教师", null),
                ("\t", null),
                ("课程地点".PadRight(8,' '), null),
                ("\t", null)

            };
            for (var i = 1; i <= maxWeek; i++)
            {
                outList.Add(($"{i} ".PadLeft(4, '0'), null));
            }

            Io?.WriteLine(outList, IoInterface.OutputType.ListTitle);
            for (var i = 0; i < Schedule.Entries.Count; i++)
            {
                ShowScheduleEntry(i, maxWeek, Schedule.Entries[i]);
            }
        }
        public void LoadXls(string path)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (!File.Exists(path))
            {
                Io?.WriteLine("未找到文件。", IoInterface.OutputType.Error);
                return;
            }

            using var fs = File.OpenRead(path);
            Schedule = new Schedule(fs);
        }
        public void LoadJson(string path)
        {
            if (!File.Exists(path))
            {
                Io?.WriteLine("未找到文件。", IoInterface.OutputType.Error);
                return;
            }
            Schedule = DeserializeObject<Schedule>(File.ReadAllText(path));
        }
        public void New()
        {
            if (!(int.TryParse(
                    Io?.ReadLine("输入年份", "1", null), out var year)
                && year >= 2020 && year <= 2021))
            {
                Io?.WriteLine("无效输入。", IoInterface.OutputType.Error);
                return;
            }
            Io?.WriteLine("选择学期：", IoInterface.OutputType.ListTitle);
            Io?.AppendWriteLinePrefix();
            Io?.WriteLine("0. 春(默认)");
            Io?.WriteLine("1. 夏");
            Io?.WriteLine("2. 秋");
            Io?.SubtractWriteLinePrefix();
            if (!int.TryParse(
                    Io?.ReadLine("", "0", null), out var s) || s > 2 || s < 0)
            {
                Io?.WriteLine("无效输入。", IoInterface.OutputType.Error);
                return;
            }

            Schedule = new Schedule(year, (Semester)s);
        }

        public void Init()
        {
            Io?.WriteLine("课表尚未初始化，您可以：", IoInterface.OutputType.ListTitle);
            Io?.AppendWriteLinePrefix();
            //Io?.WriteLine("0. 自动导入(默认)");
            Io?.WriteLine("1. 手动导入XLS(默认)");
            Io?.WriteLine("2. 手动导入JSON");
            Io?.WriteLine("3. 创建新的课表");
            Io?.WriteLine("其它. 退出");
            Io?.SubtractWriteLinePrefix();
            if (int.TryParse(Io?.ReadLine("选择", "1", null), out var o))
            {
                switch (o)
                {
                    //case 0:
                    //    Auto();
                    //    break;
                    case 1:
                        LoadXls(Io?.ReadLine("Xls位置"));
                        break;
                    case 2:
                        LoadJson(Io?.ReadLine("Json位置"));
                        break;
                    case 3:
                        New();
                        break;
                    default:
                        Environment.Exit(0);
                        break;
                }
            }
            else
            {
                Environment.Exit(0);
            }

        }

        public void Save(string path = "")
        {
            ScheduleCheck();
            if (Schedule is null) return;
            if (path == "")
                path = Io?.ReadLine("输入保存JSON文件位置");
            try
            {
                File.WriteAllText(path, SerializeObject(Schedule));
            }
            catch
            {
                Io?.WriteLine("写入错误。", IoInterface.OutputType.Error);
                Environment.Exit(0);
            }
        }
        private void ScheduleCheck()
        {
            if (Schedule is null) Init();
        }
        private void ShowScheduleEntry(int index, int maxWeek, ScheduleEntry entry)
        {
            var outList = new List<(string, ConsoleColor?)>
            {
                (index.ToString(),null),
                ("\t", null),
                (entry.CourseName.PadRight(22,' '), null),
                ("\t", null),
                (entry.DayOfWeekName, null),
                ("\t\t", null),
                (entry.StartTime.ToString(), null),
                ("\t", null),
                (entry.Length.ToString(),null),
                ("\t", null),
                (entry.Teacher, null),
                ("\t\t", null),
                (entry.Location.PadRight(8,' '), null),
                ("\t", null)

            };
            var week = 1;
            for (var i = entry.Week >> 1; week <= maxWeek; i >>= 1, week++)
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
