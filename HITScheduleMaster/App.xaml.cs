using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Windows;
using Newtonsoft.Json;

namespace HCGStudio.HITScheduleMaster
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //检查更新
            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var http = new HttpClient();
            http.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_5) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.1.1 Safari/605.1.15");
            var info = JsonConvert.DeserializeObject<dynamic>(await (await http.GetAsync(
                    "https://api.github.com/repos/HCG-Studio/HIT-Schedule-Master/releases/latest")).Content
                .ReadAsStringAsync());
            if (string.Compare(currentVersion, info.tag_name as string, StringComparison.Ordinal) >= 0) return;
            if (MessageBox.Show("有新版本发布，是否更新？", "新版本！", MessageBoxButton.YesNo, MessageBoxImage.Asterisk) ==
                MessageBoxResult.Yes)
                Process.Start(info.html_url as string ?? throw new InvalidOperationException());
        }
    }
}