using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Leo.WindowModels
{
    public partial class MessageBox
    {
        public int Results;
        public bool IsOpened;
        
        public MessageBox()
        {
            InitializeComponent();
            Hide();
        }

        public enum MessageBoxType
        {
            Info,
            Error
        }
        
        public enum MessageBoxButtons
        {
            Ok,
            OkCancel
        }

        public void showMessage(string label, string message, Enum type, Enum buttons)
        {
            Message.Text = message;
            Header.Content = label;
            IsOpened = true;

            switch (type.ToString())
            {
                case "Error":
                    Image.Source = new BitmapImage(new Uri(@"pack://application:,,,/Assets/images/error.png"));
                    System.Media.SystemSounds.Hand.Play();
                    break;
                case "Info":
                    Image.Source = new BitmapImage(new Uri(@"pack://application:,,,/Assets/images/info.png"));
                    break;
                case "Warn":
                    Image.Source = new BitmapImage(new Uri(@"pack://application:,,,/Assets/images/warn.png"));
                    System.Media.SystemSounds.Exclamation.Play();
                    break;
            }

            var ft = new FormattedText(message, 
                CultureInfo.CurrentCulture, 
                0, 
                new Typeface("Montserrat Alternates"), 
                12, 
                Brushes.White, 
                96);
            
            LeoMessageBox.Height += ft.Height;

            if (buttons.ToString() == "Ok")
            {
                CancelButton.Visibility = Visibility.Hidden;
            }
            
            Show();
        }

        private enum Operations
        {
            Close,
            None
        }
        
        private void opacityAnimation(string target, double at, double to, double time, Enum operation)
        {
            if (Properties.Settings.Default.allowOpacity)
            {
                var storyboardFade = new Storyboard();
                var animation = new DoubleAnimation(at, to, new Duration(TimeSpan.FromSeconds(time)));
                
                switch (operation.ToString())
                {
                    case "Close":
                        animation.Completed += closeApplication;
                        break;
                }

                Storyboard.SetTargetName(animation, target);
                Storyboard.SetTargetProperty(animation, new PropertyPath(OpacityProperty));
                
                storyboardFade.FillBehavior = FillBehavior.Stop;
                storyboardFade.Children.Add(animation);

                storyboardFade.Begin(this);
            } 
            else
            {
                switch (operation.ToString())
                {
                    case "Close":
                        Close();
                        break;
                }
            }
        }
        
        private void closeApplication(object? sender, EventArgs e)
        {
            IsOpened = false;
            Hide();
        }
        
        private void movingWindow(object sender, MouseButtonEventArgs e) 
        {
            DragMove();     
        }
        
        private void closeWindow(object sender, MouseButtonEventArgs e)
        {
            opacityAnimation(Name, 1, 0, 0.3, Operations.Close);
        }
        
        private void closeBtnMouseEnter(object sender, MouseEventArgs e)
        {
            opacityAnimation(CloseBackground.Name, 0, 1, 0.1, Operations.None);
            CloseBackground.Opacity = 1;
        }

        private void closeBtnMouseLeave(object sender, MouseEventArgs e)
        {
            opacityAnimation(CloseBackground.Name, 1, 0, 0.1, Operations.None);
            CloseBackground.Opacity = 0;
        }
        
        private void okBtnMouseEnter(object sender, MouseEventArgs e)
        {
            OkText.Foreground = Brushes.White;
            opacityAnimation(Ok.Name, 1, 0, 0.1, Operations.None);
            Ok.Opacity = 0;
        }

        private void okBtnMouseLeave(object sender, MouseEventArgs e)
        {
            OkText.Foreground = Brushes.Black;
            opacityAnimation(Ok.Name, 0, 1, 0.1, Operations.None);
            Ok.Opacity = 1;
        }
        private void okMouseDown(object sender, MouseEventArgs e)
        {
            Results = 1;
            opacityAnimation(Name, 1, 0, 0.3, Operations.Close);
        }
        
        private void cancelBtnMouseEnter(object sender, MouseEventArgs e)
        {
            CancelText.Foreground = Brushes.White;
            opacityAnimation(Cancel.Name, 1, 0, 0.1, Operations.None);
            Cancel.Opacity = 0;
        }

        private void cancelBtnMouseLeave(object sender, MouseEventArgs e)
        {
            CancelText.Foreground = Brushes.Black;
            opacityAnimation(Cancel.Name, 0, 1, 0.1, Operations.None);
            Cancel.Opacity = 1;
        }
        private void cancelMouseDown(object sender, MouseEventArgs e)
        {
            Results = 0;
            opacityAnimation(Name, 1, 0, 0.3, Operations.Close);
        }
    }
}