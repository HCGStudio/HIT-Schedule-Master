using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Input;

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

        }
        private void NotifyTime_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => e.Handled = (new Regex("[^0-9]+")).IsMatch(e.Text);
        //Ensure the input is a positive integer
    }
}
