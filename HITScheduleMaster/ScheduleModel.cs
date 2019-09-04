using Ical.Net;
using Ical.Net.Serialization;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HCGStudio.HITScheduleMaster
{
    public class ScheduleModel
    {
        public ObservableCollection<Course> Courses { get; set; }

        //把 ObservableCollection<Course> 转成 Calendar
        public Calendar ToCalendar()
        {
            throw new NotImplementedException();
        }


        public override string ToString()
            => new CalendarSerializer().SerializeToString(ToCalendar());
        
        public void  Export()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "iCalendar 文件 (*.ics)|*.ics",
                Title = "保存文件",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = $"课表.ics"
            };

            dialog.ShowDialog();

            try
            {
                File.WriteAllText(dialog.FileName, ToString(), new UTF8Encoding(false));
            }
            catch
            {
                MessageBox.Show("写入出错！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (MessageBox.Show("成功，您是否要了解如何将iCalendar导入到您的日历中？", "导出成功", MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Process.Start("explorer", "https://github.com/HCGStudio/HIT-Schedule-Master/wiki");
            }

        }
    }
}
