using HCGStudio.HITScheduleMaster.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HCGStudio.HITScheduleMaster
{
    public class LoginInformation:INotifyPropertyChanged
    {
        private string studentID, password, captcha;
        private ImageSource captchaImage;
        public string StudentID
        {
            set
            {
                studentID = value;
                NotifyPropertyChange();
            }
            get => studentID;
        }
        //public string Password
        //{
        //    set
        //    {
        //        password = value;
        //        NotifyPropertyChange();
        //    }
        //    get => password;
        //}
        public string Captcha
        {
            set
            {
                captcha = value;
                NotifyPropertyChange();
            }
            get => captcha;
        }
        public ImageSource CaptchaImage
        {
            set
            {
                captchaImage = value;
                NotifyPropertyChange();
            }
            get => captchaImage;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChange([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
