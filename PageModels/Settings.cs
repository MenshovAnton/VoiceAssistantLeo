using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using IWshRuntimeLibrary;
using Leo.Classes;
using Leo.WindowModels;
using Microsoft.Win32;
using Application = System.Windows.Forms.Application;
using File = System.IO.File;
using MessageBox = Leo.WindowModels.MessageBox;

namespace Leo.PageModels
{
    public partial class Settings
    {
        public static float VoiceVolume = Properties.Settings.Default.voiceVol;
        public static float SoundVolume = Properties.Settings.Default.soundVol;

        private readonly MediaPlayer _player = new();
        private readonly Logger _logger = new();
        private readonly MessageBox _messageBox = new();

        private static readonly string ShortcutPath = 
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Start Menu\Programs\Startup\Лео.lnk";


        public Settings()
        {
            InitializeComponent();
            
            DevModeBox.IsChecked = Properties.Settings.Default.isDevModeTrue;
            MinimizeToTrayBox.IsChecked = Properties.Settings.Default.isMinimizeToTrayTrue;
            AutoRunBox.IsChecked = Properties.Settings.Default.isAutoRun;
            OpacityBox.IsChecked = Properties.Settings.Default.allowOpacity;
            
            AppStartBox.IsChecked = Properties.Settings.Default.allowProgrammsStart;
            BrowserStartBox.IsChecked = Properties.Settings.Default.allowBrowserStart;
            UsingNetworkBox.IsChecked = Properties.Settings.Default.allowNetworkUsing;
            ComputerControlBox.IsChecked = Properties.Settings.Default.allowComputerControl;
            
            VoiceVolumeSlider.Value = Properties.Settings.Default.voiceVol;
            SoundVolumeSlider.Value = Properties.Settings.Default.soundVol;
            
            NotSaveMessageBox.IsChecked = Properties.Settings.Default.notSaveMessages;
            OffLotMessageWarnBox.IsChecked = Properties.Settings.Default.offLotMessageWarn;

            using var key = Registry.CurrentUser.OpenSubKey("Software\\AssistantLeo");
            ProgramLanguageComboBox.SelectedIndex = (int)key?.GetValue("Language")!;
        }

        private void voiceVolumeSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VoiceVolume = (float)VoiceVolumeSlider.Value;

            Properties.Settings.Default.voiceVol = VoiceVolume;
            Properties.Settings.Default.Save();
        }

        private void voiceVolumeTest(object sender, MouseEventArgs e)
        {
            _player.Open(new Uri(@".\voices\test.wav", UriKind.Relative));
            _player.Volume = VoiceVolume / 100.0f;
            _player.Play();
        }

        private void soundVolumeSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SoundVolume = (float)SoundVolumeSlider.Value;

            Properties.Settings.Default.soundVol = SoundVolume;
            Properties.Settings.Default.Save();
        }

        private void soundVolumeTest(object sender, MouseEventArgs e)
        {
            _player.Open(new Uri(@".\sounds\start.wav", UriKind.Relative));
            _player.Volume = SoundVolume / 100.0f;
            _player.Play();
        }

        private void devModeBoxChecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.isDevModeTrue = true;
            Properties.Settings.Default.Save();
        }
        private void devModeBoxUnchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.isDevModeTrue = false;
            Properties.Settings.Default.Save();
        }

        private void minimizeToTrayBoxChecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.isMinimizeToTrayTrue = true;
            Properties.Settings.Default.Save();
        }
        private void minimizeToTrayBoxUnchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.isMinimizeToTrayTrue = false;
            Properties.Settings.Default.Save();
        }

        private void appStartBoxChecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowProgrammsStart = true;
            Properties.Settings.Default.Save();
        }
        private void appStartBoxUnchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowProgrammsStart = false;
            Properties.Settings.Default.Save();
        }

        private void browserStartBoxChecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowBrowserStart = true;
            Properties.Settings.Default.Save();
        }
        private void browserStartBoxUnchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowBrowserStart = false;
            Properties.Settings.Default.Save();
        }

        private void usingNetworkBoxChecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowNetworkUsing = true;
            Properties.Settings.Default.Save();
        }
        private void usingNetworkBoxUnchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowNetworkUsing = false;
            Properties.Settings.Default.Save();
        }

        private void computerControlBoxChecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowComputerControl = true;
            Properties.Settings.Default.Save();
        }
        private void computerControlBoxUnchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowComputerControl = false;
            Properties.Settings.Default.Save();
        }

        private void opacityBoxChecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowOpacity = true;
            Properties.Settings.Default.Save();
        }
        private void opacityBoxUnchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowOpacity = false;
            Properties.Settings.Default.Save();
        }

        private void addToAutoRun(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.isAutoRun = true;
            Properties.Settings.Default.Save();
            const string message = "Не удалось создать запись в реестре\n\nКод ошибки: 02";
            const string label = "Что-то пошло не так...";

            try
            {
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(ShortcutPath);
                shortcut.TargetPath = Application.ExecutablePath;
                shortcut.WorkingDirectory = Path.GetDirectoryName(Application.ExecutablePath);
                shortcut.Save();
            }
            catch (Exception ex)
            {
                _messageBox.showMessage(label, message, MessageBox.MessageBoxType.Error,
                    MessageBox.MessageBoxButtons.Ok);
                _logger.error("Leo was unable to add himself from startup.");
                _logger.error(ex.Message);
            }

        }

        private void removeToAutoRun(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.isAutoRun = false;
            Properties.Settings.Default.Save();
            
            const string message = "Не удалось изменить/удалить запись в реестре\n\nКод ошибки: 03";
            const string label = "Что-то пошло не так...";
            
            try
            {
                File.Delete(ShortcutPath);
            }
            catch
            {
                _messageBox.showMessage(message, label, MessageBox.MessageBoxType.Error, MessageBox.MessageBoxButtons.Ok);
                _logger.error("Leo was unable to remove himself from startup.");
            }

        }

        private void openLogs(object sender, RoutedEventArgs e)
        { Process.Start("explorer.exe", @".\Logs"); }

        private void clearMessages(object sender, RoutedEventArgs e)
        {
            ChatManager chatManager = new();
            chatManager.clearChat();
        }
        
        private void voskSettings(object sender, RoutedEventArgs e)
        { MainWindow.getVoskSettingsPage(); }

        private void settingsReset(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.isDevModeTrue = false;
            Properties.Settings.Default.isMinimizeToTrayTrue = false;
            Properties.Settings.Default.isAutoRun = false;
            Properties.Settings.Default.allowOpacity = true;

            Properties.Settings.Default.allowProgrammsStart = true;
            Properties.Settings.Default.allowBrowserStart = true;
            Properties.Settings.Default.allowNetworkUsing = true;
            Properties.Settings.Default.allowComputerControl = true;

            Properties.Settings.Default.voiceVol = 100.0f;
            Properties.Settings.Default.soundVol = 100.0f;

            Properties.Settings.Default.notSaveMessages = false;
            Properties.Settings.Default.offLotMessageWarn = false;
            
            Properties.Settings.Default.Save();

            DevModeBox.IsChecked = Properties.Settings.Default.isDevModeTrue;
            MinimizeToTrayBox.IsChecked = Properties.Settings.Default.isMinimizeToTrayTrue;
            AutoRunBox.IsChecked = Properties.Settings.Default.isAutoRun;
            OpacityBox.IsChecked = Properties.Settings.Default.allowOpacity;
            
            AppStartBox.IsChecked = Properties.Settings.Default.allowProgrammsStart;
            BrowserStartBox.IsChecked = Properties.Settings.Default.allowBrowserStart;
            UsingNetworkBox.IsChecked = Properties.Settings.Default.allowNetworkUsing;
            ComputerControlBox.IsChecked = Properties.Settings.Default.allowComputerControl;
            
            VoiceVolumeSlider.Value = Properties.Settings.Default.voiceVol;
            SoundVolumeSlider.Value = Properties.Settings.Default.soundVol;
            
            NotSaveMessageBox.IsChecked = Properties.Settings.Default.notSaveMessages;
            OffLotMessageWarnBox.IsChecked = Properties.Settings.Default.offLotMessageWarn;
        }
        
        private void notSaveMessagesBoxChecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.notSaveMessages = true;
            Properties.Settings.Default.Save();
        }

        private void notSaveMessagesBoxUnchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.notSaveMessages = false;
            Properties.Settings.Default.Save();
        }
        
        private void offLotMessageWarnBoxChecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.offLotMessageWarn = true;
            Properties.Settings.Default.Save();
        }

        private void offLotMessageWarnBoxUnchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.offLotMessageWarn = false;
            Properties.Settings.Default.Save();
        }

        private void changeLanguage(object sender, SelectionChangedEventArgs e)
        { Registry.CurrentUser.CreateSubKey("Software\\AssistantLeo").SetValue("Language", ProgramLanguageComboBox.SelectedIndex); }
        
        private void programLanguageComboboxClose(object sender, EventArgs e)
        { _messageBox.showMessage("Уведомление","Перезагрузите приложение для вступления изменений в силу", 
                MessageBox.MessageBoxType.Error, MessageBox.MessageBoxButtons.Ok); }
    }
}
