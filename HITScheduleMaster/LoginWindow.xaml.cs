using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HCGStudio.HITScheduleMaster.Annotations;

namespace HCGStudio.HITScheduleMaster
{
    /// <summary>
    ///     WinLogin.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window, INotifyPropertyChanged
    {
        private ImageSource _captchaImage;

        public LoginWindow()
        {
            InitializeComponent();
            DataContext = this;
            Http = new HttpClient(new HttpClientHandler
            {
                UseCookies = true,
                AllowAutoRedirect = true,
                CookieContainer = new CookieContainer()
            });
            //不要问为啥User-Agent是这玩意
            Http.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_5) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.1.1 Safari/605.1.15");
            Http.BaseAddress = new Uri("http://jwts.hit.edu.cn/");
        }

        public ImageSource CaptchaImage
        {
            get => _captchaImage;
            set
            {
                _captchaImage = value;
                NotifyPropertyChange();
            }
        }


        private HttpClient Http { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void LoginWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            //检测是否处于校园网环境
            try
            {
                var fetchIsInSchool = await Http.GetAsync("/");
                if (!fetchIsInSchool.IsSuccessStatusCode)
                {
                    MessageBox.Show("您需要在校园网中使用本程序", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(0);
                }
            }
            catch
            {
                MessageBox.Show("您需要在校园网中使用本程序", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }

            await GetAndUpdateCaptcha();
        }

        private async Task GetAndUpdateCaptcha()
        {
            var captchaFileName = Path.GetTempFileName();
            var fetchCode = await Http.GetAsync("/captchaImage");
            var writeCaptcha = new FileStream(captchaFileName, FileMode.Create);
            await (await fetchCode.Content.ReadAsStreamAsync()).CopyToAsync(writeCaptcha);
            await writeCaptcha.FlushAsync();
            writeCaptcha.Close();
            CaptchaImage = new BitmapImage(new Uri(captchaFileName));
        }

        private async void Login_Button_OnClick(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            using var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("usercode", IdBox.Text),
                new KeyValuePair<string, string>("password", PasswordBox.Password),
                new KeyValuePair<string, string>("code", CaptchaBox.Text)
            });
            var response = await Http.PostAsync("/loginLdap", content);
            if (response.RequestMessage.RequestUri.ToString() != "http://jwts.hit.edu.cn/loginLdap")
            {
                MessageBox.Show("学号、密码或者验证码输入错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                await GetAndUpdateCaptcha();
                IsEnabled = true;
            }
            else
            {
                new ExportWindow {Http = Http}.ShowDialog();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChange([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void Image_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            await GetAndUpdateCaptcha();
        }

        private void Exit_Base_OnClick(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}