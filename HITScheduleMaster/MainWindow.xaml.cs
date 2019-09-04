using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Diagnostics;
using Microsoft.Win32;

namespace HCGStudio.HITScheduleMaster
{
    public partial class MainWindow
    {
        ScheduleModel Model;
        public MainWindow()
        {
            InitializeComponent();
            Model = new ScheduleModel();
            DataContext = Model;
            

        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
            => Environment.Exit(0);

        private void RideOn_OnClick(object sender, RoutedEventArgs e)//QuickExport
            => Model.Export();

        private void RemoveCourses_OnClick(object sender, RoutedEventArgs e)
        {
            if (CourseView.SelectedItems.Count!=0)
            {
                foreach (var item in CourseView.SelectedItems)
                {
                    Model.Courses.Remove((Course)item);
                }
            }
        }

        private void ImportCourses_OnClick(object sender, RoutedEventArgs e)
        {

        }
        private void ExportCourses_OnClick(object sender, RoutedEventArgs e)
        {
            
            if (CourseView.SelectedItems.Count != 0)
            {
                
                
                var dialog = new SaveFileDialog
                {
                    Filter = "JSON 文件 (*.json)|*.json",
                    Title = "保存文件",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    FileName = $"课表.ics"
                };

                dialog.ShowDialog();

                try
                {
                    File.WriteAllText(dialog.FileName, 
                        JsonConvert.SerializeObject(CourseView.SelectedItems), 
                        new UTF8Encoding(false));
                }
                catch
                {
                    MessageBox.Show("写入出错！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (MessageBox.Show("成功，您是否要了解如何将分享或重利用导出的课程？", "导出成功", MessageBoxButton.YesNo,
                        MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Process.Start("explorer", "https://github.com/HCGStudio/HIT-Schedule-Master/wiki");
                }


            }
        }
        private void NotifyTime_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => e.Handled = (new Regex("[^0-9]+")).IsMatch(e.Text);
        //Ensure the input is a positive integer
    }
}
