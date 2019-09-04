using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using ExcelDataReader;
using HtmlAgilityPack;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Microsoft.Win32;
using Trigger = Ical.Net.DataTypes.Trigger;

namespace HCGStudio.HITScheduleMaster
{
    /// <summary>
    ///     WinExport.xaml 的交互逻辑
    /// </summary>
    public partial class ExportWindow : Window
    {
        public ExportWindow()
        {
            InitializeComponent();
        }

        public HttpClient Http { get; set; }

        private void OtherClassSchedule_OnChecked(object sender, RoutedEventArgs e)
        {
            ClassNumberInput.Visibility = Visibility.Visible;
        }

        private void OtherClassSchedule_OnUnchecked(object sender, RoutedEventArgs e)
        {
            ClassNumberInput.Visibility = Visibility.Collapsed;
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }


        private async void OnRideOn(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            var saveDialog = new SaveFileDialog
            {
                Filter = "iCalendar 文件 (*.ics)|*.ics",
                Title = "保存文件",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            var fetchSemester = await Http.GetAsync("/kbcx/queryGrkb");
            var fetched = await fetchSemester.Content.ReadAsStringAsync();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(fetched);
            var select = htmlDoc.DocumentNode.SelectSingleNode("//option[@selected]");
            var semester = Regex.Match(select.OuterHtml, @"\d{4}-\d{5}").Value;
            var tempName = Regex.Match(htmlDoc.DocumentNode.SelectSingleNode("//span[@class='ml10 bold']").InnerHtml,
                @"[\u4e00-\u9fa5]{1,}课表").Value;
            var name = tempName.Substring(0, tempName.Length - 2);

            //都9012年了，某些网站仍然在使用非UTF-8编码
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Calendar cal;

            if (PersonalSchedule.IsChecked == true)
            {
                using var content =
                    new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("xnxq", semester),
                        new KeyValuePair<string, string>("fhlj", "kbcx/queryGrkb")
                    });
                var getSchedule = await Http.PostAsync("/kbcx/ExportGrKbxx", content);
                await using var xlsStream = await getSchedule.Content.ReadAsStreamAsync();
                cal = ConvertPersonalXlsToCalendar(xlsStream, semester);
                saveDialog.FileName = $"{name}个人课表.ics";
            }
            else if (RecommendSchedule.IsChecked == true)
            {
                using var content =
                    new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("xnxq", semester),
                        new KeyValuePair<string, string>("zc", string.Empty)
                    });
                var getSchedule = await Http.PostAsync("/kbcx/ExportBjKbxx", content);
                await using var xlsStream = await getSchedule.Content.ReadAsStreamAsync();
                cal = ConvertRecommendXlsToCalendar(xlsStream, semester);
                saveDialog.FileName = $"{name}班级推荐课表.ics";
            }
            else
            {
                //防止报错
                cal = new Calendar();
            }

            saveDialog.ShowDialog(this);

            try
            {
                File.WriteAllText(saveDialog.FileName, new CalendarSerializer().SerializeToString(cal),
                    new UTF8Encoding(false));
            }
            catch
            {
                MessageBox.Show("写入出错！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }

            if (MessageBox.Show("成功，您是否要了解如何将iCalendar导入到您的日历中？", "导出成功", MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Process.Start("explorer", "https://github.com/HCGStudio/HIT-Schedule-Master/wiki");
                Environment.Exit(0);
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private Calendar ConvertPersonalXlsToCalendar(Stream xlsStream, string semester)
        {
            var cal = new Calendar();

            var reader = ExcelReaderFactory.CreateReader(xlsStream);
            var table = reader.AsDataSet().Tables[0];

            for (var col = 3 - 1; col < 8 - 1; ++col)
            for (var row = 3 - 1; row < 9 - 1; ++row)
            {
                //col - 1 : sequent
                //row - 1 : weekday
                var current = table.Rows[col][row] as string;
                if (string.IsNullOrWhiteSpace(current))
                    continue;
                var splitCurrent = current.Split("</br>");
                for (var index = 0; (index + 1) * 2 <= splitCurrent.Length; ++index)
                {
                    var name = splitCurrent[index * 2];
                    var getLocation = splitCurrent[index * 2 + 1].Split("]周");
                    var location = getLocation.Length == 1 ? string.Empty : getLocation.Last();
                    var weeks = splitCurrent[index * 2 + 1].Split("[").Last().Split("]周").First().Replace("，", "")
                        .Split(", ");
                    var teacher = splitCurrent[index * 2 + 1].Split("[").First();
                    var weekList = new List<int>();
                    foreach (var week in weeks)
                        if (Regex.IsMatch(week, @"\d{1,}-\d{1,}单"))
                        {
                            var contains = Regex.Matches(week, @"\d+");
                            var from = Convert.ToInt32(contains[0].Value);
                            var to = Convert.ToInt32(contains[1].Value);
                            while (from <= to)
                            {
                                if (from % 2 == 1)
                                    weekList.Add(from);
                                from++;
                            }
                        }
                        else if (Regex.IsMatch(week, @"\d{1,}-\d{1,}双"))
                        {
                            var contains = Regex.Matches(week, @"\d+");
                            var from = Convert.ToInt32(contains[0].Value);
                            var to = Convert.ToInt32(contains[1].Value);
                            while (from <= to)
                            {
                                if (from % 2 == 0)
                                    weekList.Add(from);
                                from++;
                            }
                        }
                        else if (Regex.IsMatch(week, @"\d{1,}-\d{1,}"))
                        {
                            var contains = Regex.Matches(week, @"\d+");
                            var from = Convert.ToInt32(contains[0].Value);
                            var to = Convert.ToInt32(contains[1].Value);
                            while (from <= to)
                            {
                                weekList.Add(from);
                                from++;
                            }
                        }
                        else
                        {
                            weekList.Add(Convert.ToInt32(week));
                        }

                    foreach (var e in weekList.Select(i => new CalendarEvent
                    {
                        Location = location,
                        Start = new CalDateTime(ReferenceTime.BaseTime[semester].AddDays((i - 1) * 7 + row - 2) +
                                                ReferenceTime.StartTimes[col - 1]),
                        Duration = ReferenceTime.ClassLength,
                        Summary = $"{name} by {teacher} @ {location}"
                    }))
                    {
                        e.Alarms.Add(new Alarm
                        {
                            Summary = $"您在{location}有一节{name}课程即将开始。",
                            Action = AlarmAction.Display,
                            Trigger = new Trigger(TimeSpan.FromMinutes(-25))
                        });
                        cal.Events.Add(e);
                    }
                }
            }

            return cal;
        }

        private Calendar ConvertRecommendXlsToCalendar(Stream xlsStream, string semester)
        {
            var cal = new Calendar();

            var reader = ExcelReaderFactory.CreateReader(xlsStream);
            var table = reader.AsDataSet().Tables[0];

            for (var col = 3 - 1; col < 8 - 1; ++col)
            for (var row = 3 - 1; row < 9 - 1; ++row)
            {
                //col - 1 : sequent
                //row - 1 : weekday
                var current = table.Rows[col][row] as string;
                if (string.IsNullOrWhiteSpace(current))
                    continue;
                var splitCurrent = current.Split("</br>");
                foreach (var currentCourse in splitCurrent)
                {
                    var name = currentCourse.Split("◇").First();
                    var getLocation = currentCourse.Split("◇");
                    var location = getLocation.Length == 1 ? string.Empty : getLocation.Last();
                    var b = currentCourse.Split("[").Last();
                    var weeks = b.Split("周]").First().Replace("，", ", ").Split(", ");
                    var teacher = currentCourse.Split("◇")[1].Split("[").First();
                    var weekList = new List<int>();
                    foreach (var week in weeks)
                        if (Regex.IsMatch(week, @"\d{1,}-\d{1,}单"))
                        {
                            var contains = Regex.Matches(week, @"\d+");
                            var from = Convert.ToInt32(contains[0].Value);
                            var to = Convert.ToInt32(contains[1].Value);
                            while (from <= to)
                            {
                                if (from % 2 == 1)
                                    weekList.Add(from);
                                from++;
                            }
                        }
                        else if (Regex.IsMatch(week, @"\d{1,}-\d{1,}双"))
                        {
                            var contains = Regex.Matches(week, @"\d+");
                            var from = Convert.ToInt32(contains[0].Value);
                            var to = Convert.ToInt32(contains[1].Value);
                            while (from <= to)
                            {
                                if (from % 2 == 0)
                                    weekList.Add(from);
                                from++;
                            }
                        }
                        else if (Regex.IsMatch(week, @"\d{1,}-\d{1,}"))
                        {
                            var contains = Regex.Matches(week, @"\d+");
                            var from = Convert.ToInt32(contains[0].Value);
                            var to = Convert.ToInt32(contains[1].Value);
                            while (from <= to)
                            {
                                weekList.Add(from);
                                from++;
                            }
                        }
                        else
                        {
                            weekList.Add(Convert.ToInt32(week));
                        }

                    foreach (var e in weekList.Select(i => new CalendarEvent
                        {
                            Location = location,
                            Start = new CalDateTime(ReferenceTime.BaseTime[semester].AddDays((i - 1) * 7 + row - 2) +
                                                    ReferenceTime.StartTimes[col - 1]),
                            Duration = ReferenceTime.ClassLength,
                            Summary = $"{name} by {teacher} @ {location}"
                        }))
                        //课表为班级推荐课表的时候默认禁止通知

                        //e.Alarms.Add(new Alarm
                        //{
                        //    Summary = $"您在{location}有一节{name}课程即将开始。",
                        //    Action = AlarmAction.Display,
                        //    Trigger = new Ical.Net.DataTypes.Trigger(TimeSpan.FromMinutes(-25))
                        //});
                        cal.Events.Add(e);
                }
            }

            return cal;
        }
    }
}