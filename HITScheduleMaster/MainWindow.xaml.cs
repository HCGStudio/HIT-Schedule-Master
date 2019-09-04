using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;

namespace HCGStudio.HITScheduleMaster
{
    public partial class MainWindow
    {
        ScheduleModel Model;
        public MainWindow()
        {
            InitializeComponent();
            Model = new ScheduleModel();
            this.DataContext = Model;
            

        }

        private void Login_Button_OnClick(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
