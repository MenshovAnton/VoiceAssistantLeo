using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Leo.Classes;
using Leo.PageModels;
using Microsoft.Win32;
using static Leo.PageModels.Chat;
using Control = System.Windows.Forms.Control;
using Image = System.Windows.Controls.Image;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace Leo.WindowModels
{
    public partial class MainWindow
    {
        public static ObservableCollection<Messages>? ChatCollection { get; set; }
        public static bool MicAccess = true;

        private static readonly ChatManager ChatManager = new();
        private static MainWindow? Instance { get; set; }

        private static object _previousPage = new Home();

        public MainWindow()
        {
            using var key = Registry.CurrentUser.OpenSubKey("Software\\AssistantLeo");
            if ((int)key?.GetValue("Language")! == 0)
            { Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU"); }
            if ((int)key?.GetValue("Language")! == 1)
            { Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US"); }

            InitializeComponent();
            
            if (CultureInfo.CurrentUICulture.Name == "en-US")
            {
                VersionLabel.Margin = new Thickness(130, 6, 0, 0);
            }
            
            getChatPage(this, null);
            ChatManager.deserializeChat();
            getHomePage(this, null);
            Classes.Vosk.main();

            if (Properties.Settings.Default.isMuted)
            {
                Mute.Content = (Image)TryFindResource("MuteImage");
                TrayIconMuteBtn.Header = "Вкл. микрофон";
            }
            else
            {
                Mute.Content = (Image)TryFindResource("MicrophoneImage");
                TrayIconMuteBtn.Header = "Выкл. микрофон";
            }

            Classes.Vosk.update();
            ChatCollection = new ObservableCollection<Messages>();
            Console.WriteLine(@"(C) Copyright Menshov Anton Romanovich (WaysoonProgramms) 2023-2024");

            if (MicAccess == false)
            {
                Mute.Content = (Image)TryFindResource("MuteImage");
                Mute.Opacity = 0.5;
                Mute.IsEnabled = false;
                
                TrayIconMuteBtn.Header = "Вкл. микрофон";
                TrayIconMuteBtn.IsEnabled = false;
                TrayIconMuteBtn.Opacity = 0.5;
            }
            
            Instance = this;
        }
        
        private void windowLoaded(object sender, RoutedEventArgs e)
        { opacityAnimation(Name, 0, 1, 0.1, 2); }

        private void trayIconClick(object sender, RoutedEventArgs e)
        {
            if (Equals(sender, TrayIconChatBtn))
            {
                getChatPage(TrayIconChatBtn, null);
            }
            if (Equals(sender, TrayIconSettingsBtn))
            {
                getSettingsPage(TrayIconSettingsBtn, null);
            }


            Show();
            WindowState = WindowState.Normal;

            opacityAnimation(Name, 0, 1, 0.3, 2);
        }

        private void trayIconMute(object sender, RoutedEventArgs e)
        {
            if (MicAccess == false)
            {
                Classes.Vosk.error1();
                return;
            }

            if (Properties.Settings.Default.isMuted)
            {
                Properties.Settings.Default.isMuted = false;
                Properties.Settings.Default.Save();

                Mute.Content = (Image)TryFindResource("MicrophoneImage");
                TrayIconMuteBtn.Header = "Выкл. микрофон";
            }
            else
            {
                Properties.Settings.Default.isMuted = true;
                Properties.Settings.Default.Save();

                Mute.Content = (Image)TryFindResource("MuteImage");
                TrayIconMuteBtn.Header = "Вкл. микрофон";
            }

            Classes.Vosk.update();
        }

        private void trayIconClose(object sender, RoutedEventArgs e)
        { Close(); }

        private void movingWindow(object sender, MouseButtonEventArgs e) 
        { DragMove(); }

        private void opacityAnimation(string target, double at, double to, double time, int operation)
        {
            if (Properties.Settings.Default.allowOpacity)
            {
                var storyboardFade = new Storyboard();
                var animation = new DoubleAnimation(at, to, new Duration(TimeSpan.FromSeconds(time)));
                
                switch (operation)
                {
                    case 0:
                        animation.Completed += animationCompletedClose;
                        break;
                    case 1:
                        animation.Completed += animationCompletedHide;
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
                switch (operation)
                {
                    case 0:
                        Close();
                        break;
                    case 1:
                        Hide();
                        break;
                }
            }
        }

        private void animationCompletedClose(object? sender, EventArgs e)
        {
            TaskbarIcon.Visibility = Visibility.Hidden;
            Close();
        }

        private void animationCompletedHide(object? sender, EventArgs e)
        {
            Hide();
        }

        private void closeWindow(object sender, EventArgs e)
        {
            opacityAnimation(Name, 1, 0, 0.1, Properties.Settings.Default.isMinimizeToTrayTrue ? 1 : 0);
        }
        
        private void minimizeWindow(object sender, EventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void mute(object sender, EventArgs? e)
        {
            if (MicAccess == false)
            {
                Classes.Vosk.error1();
                return;
            }

            if (Properties.Settings.Default.isMuted)
            {
                Properties.Settings.Default.isMuted = false;
                Properties.Settings.Default.Save();

                Mute.Content = (Image)TryFindResource("MicrophoneImage");
            } else
            {
                Properties.Settings.Default.isMuted = true;
                Properties.Settings.Default.Save();

                Mute.Content = (Image)TryFindResource("MuteImage");
            }

            Classes.Vosk.update();
        }

        private void getHomePage(object sender, MouseButtonEventArgs? e)
        {
            removeMarkers();
            MainFrame.Content = new Home();
            HomeBtnMarker.Opacity = 1;
            _previousPage = new Home();
        }
        private void getSettingsPage(object sender, MouseButtonEventArgs? e)
        {
            removeMarkers();
            MainFrame.Content = new Settings();
            SettingsBtnMarker.Opacity = 1;
            _previousPage = new Settings();
        }
        private void getChatPage(object sender, MouseButtonEventArgs? e)
        { 
            removeMarkers();
            MainFrame.Content = new Chat();
            ChatBtnMarker.Opacity = 1;
            _previousPage = new Chat();
        }
        private void getAboutPage(object sender, MouseButtonEventArgs? e)
        {
            removeMarkers();
            MainFrame.Content = new About();
            AboutBtnMarker.Opacity = 1;
            _previousPage = new About();
        }

        public static void getSkillsPage()
        { Instance!.MainFrame.Content = new Skills(); }
        public static void getVoskSettingsPage()
        { Instance!.MainFrame.Content = new VoskSettings(); }
        public static void backPage()
        { Instance!.MainFrame.Content = _previousPage; }
        private void removeMarkers()
        {
            HomeBtnMarker.Opacity = 0;
            SettingsBtnMarker.Opacity = 0;
            ChatBtnMarker.Opacity = 0;
            AboutBtnMarker.Opacity = 0;
        }
        
        private void authorLink(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo 
            { FileName = "https://github.com/WaysoonProgramms", UseShellExecute = true });
        }

        private void homeBtnMouseEnter(object sender, MouseEventArgs e)
        {
            opacityAnimation(HomeBtnFillMarker.Name, 0, 0.1, 0.1, 2);
            HomeBtnFillMarker.Opacity = 0.1;
        }
        private void homeBtnMouseLeave(object sender, MouseEventArgs e)
        {
            opacityAnimation(HomeBtnFillMarker.Name, 0.1, 0, 0.1, 2);
            HomeBtnFillMarker.Opacity = 0;
        }

        private void settingsBtnMouseEnter(object sender, MouseEventArgs e)
        {
            opacityAnimation(SettingsBtnFillMarker.Name, 0, 0.1, 0.1, 2);
            SettingsBtnFillMarker.Opacity = 0.1;
        }
        private void settingsBtnMouseLeave(object sender, MouseEventArgs e)
        {
            opacityAnimation(SettingsBtnFillMarker.Name, 0.1, 0, 0.1, 2);
            SettingsBtnFillMarker.Opacity = 0;
        }

        private void chatBtnMouseEnter(object sender, MouseEventArgs e)
        {
            opacityAnimation(ChatBtnFillMarker.Name, 0, 0.1, 0.1, 2);
            ChatBtnFillMarker.Opacity = 0.1;
        }
        private void chatBtnMouseLeave(object sender, MouseEventArgs e)
        {
            opacityAnimation(ChatBtnFillMarker.Name, 0.1, 0, 0.1, 2);
            ChatBtnFillMarker.Opacity = 0;
        }

        private void aboutBtnMouseEnter(object sender, MouseEventArgs e)
        {
            opacityAnimation(AboutBtnFillMarker.Name, 0, 0.1, 0.1, 2);
            AboutBtnFillMarker.Opacity = 0.1;
        }
        private void aboutBtnMouseLeave(object sender, MouseEventArgs e)
        {
            opacityAnimation(AboutBtnFillMarker.Name, 0.1, 0, 0.1, 2);
            AboutBtnFillMarker.Opacity = 0;
        }

        private void authorMouseEnter(object sender, MouseEventArgs e)
        {
            AuthorLine.Opacity = 1;
        }
        private void authorMouseLeave(object sender, MouseEventArgs e)
        {
            AuthorLine.Opacity = 0;
        }

        private void hotKeys(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.M)
            {
                mute(this, null);
            }

            if (e.Key == Key.L && Control.ModifierKeys == Keys.Control)
            {
                Process.Start("explorer.exe", ".\\logs");
            }
        }
    }
}