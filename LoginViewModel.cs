using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace GHCustomControls
{
    public class LoginViewModel : INotifyPropertyChanged
    {

        LoginWindow window;
        public event PropertyChangedEventHandler PropertyChanged;

        private LoginCommand loginCommand;

        public LoginCommand LoginCommand
        {
            get { return loginCommand; }
            set { 
                loginCommand = value;
                OnPropertyChange(nameof(LoginCommand));
            }
        }


       
        private string userName = "user name";

        public string UserName
        {
            get { return userName; }
            set { 
                if (userName != value)
                {
                    userName = value;
                    IsValidEmail = isValidEmail(userName) ;
                    OnPropertyChange(nameof(UserName));
                }
            }
        }

        private bool _isValidEmail = false;

        public bool IsValidEmail
        {
            get { return _isValidEmail; }
            set
            {
                if (_isValidEmail != value)
                {
                    _isValidEmail = value;
                    OnPropertyChange(nameof(IsValidEmail));
                }
            }
        }


        private string message = "Login...";

        public string Message
        {
            get { return message; }
            set {
                if (message != value)
                {
                    message = value;
                    OnPropertyChange(nameof(Message));
                }
            }
        }

        private Status status = Status.None;

        public Status Status
        {
            get { return status; }
            set
            {
                if (status != value)
                {
                    status = value;
                    OnPropertyChange(nameof(Status));
                }
            }
        }



        internal string Password
        {
            get {
                if (window == null)
                    return string.Empty;
                else
                    return window.passwordBox.Password; 
            }
             
        }

        

        public void Login(LoginCommand loginCommand)
        {
            this.LoginCommand = loginCommand;
            window = new LoginWindow();
            window.DataContext = this;
            window.ShowDialog();
            //if (window.DialogResult.HasValue && window.DialogResult.Value)
            //{
            //    password = window.passwordBox.Password;
            //    return true;
            //}
            //return false;
        }
        bool isValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
        void OnPropertyChange(string propName)
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs( propName));
        }
    }

    public abstract class LoginCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        LoginViewModel loginView;

        public LoginCommand(LoginViewModel loginView)
        {
            this.loginView = loginView;
        }
        public bool CanExecute(object parameter)
        {
            return !string.IsNullOrEmpty(loginView.UserName) && loginView.IsValidEmail;
        }

        public void  Execute(object parameter)
        {
            Task.Run(()=> Login(loginView.UserName,loginView.Password));
        }

        public abstract void Login(string username, string password);

      

    }

    

    public enum Status
    {
        Info = 0,Warning = 1,Error = 2, None=3, Completed = 4
    }

    public class StatusBrushConcerter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType.Equals(typeof(System.Windows.Media.Brush)) )
            {
                if (value == null)
                {                    
                    return System.Windows.Media.Brushes.Transparent;
                }
                if (value is Status status)
                {
                    switch(status)
                    {
                        case Status.Error:
                            return System.Windows.Media.Brushes.Red;
                        case Status.Warning:
                            return System.Windows.Media.Brushes.Orange;
                        case Status.Info:
                            return System.Windows.Media.Brushes.Green;
                        default:
                            return System.Windows.Media.Brushes.LightGray;
                    }
                }
            }
            if (targetType.Equals(typeof(bool)))
            {
                if (value == null)
                {
                    return true;
                }
                if (value is Status status)
                {
                    return status != Status.Completed;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class BoolBrushConcerter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType.Equals(typeof(System.Windows.Media.Brush)))
            {
                if (value == null)
                {
                    return System.Windows.Media.Brushes.Black;
                }
                if ((bool)value) 
                {
                    return System.Windows.Media.Brushes.Black;
                }
                else
                {
                    return System.Windows.Media.Brushes.Red;

                }
            }
             
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
