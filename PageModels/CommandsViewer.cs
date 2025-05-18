using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Leo.Classes;
using Leo.WindowModels;
using WinRT;
using MessageBox = Leo.WindowModels.MessageBox;

namespace Leo.PageModels
{
    public partial class CommandsViewer
    {
        private readonly MessageBox _messageBox = new();
        private static Dispatcher? _dispatcher;
        
        public CommandsViewer()
        {
            InitializeComponent();
            CommandsList.ItemsSource = Command.CommandsCollection;
            
            var currentDispatcher = Dispatcher.CurrentDispatcher;
            _dispatcher = currentDispatcher;
        }

        private void getCommandsEditor(object sender, RoutedEventArgs args)
        {
            MainWindow.getCommandsEditorPage();
        }
        
        private void openJson(object sender, RoutedEventArgs args)
        {
            Process.Start("notepad.exe", ".\\commands.json");
        }

        private void changeEditorButtonsStatus(bool status)
        {
            DeleteCommandButton.IsEnabled = status;
            EditCommandButton.IsEnabled = status;
        }

        private void selectedNewItem(object sender, SelectionChangedEventArgs e)
        {
            changeEditorButtonsStatus(true);
        }

        private async void deleteCommandAction(object sender, RoutedEventArgs e)
        {
            _messageBox.showMessage(Properties.Resources.messageBox_messageSign, Properties.Resources.system_message3,
                MessageBox.MessageBoxType.Info, MessageBox.MessageBoxButtons.OkCancel);
            await Task.Run(() =>
            {
                while (_messageBox.IsOpened)
                {
                    if (_messageBox.Results == 1)
                    {
                        _dispatcher?.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
                        {
                            Command.deleteCommand(CommandsList.SelectedIndex);
                            changeEditorButtonsStatus(false);
                        });
                    }
                    else
                    {
                        continue;
                    }
                    break;
                }
            });
           
        }

        private void editCommandAction(object sender, RoutedEventArgs e)
        {
            MainWindow.getCommandsEditorPage(CommandsList.SelectedIndex,
                (CommandsList.SelectedItem as CommandDataFormat)!.Name!,
                (CommandsList.SelectedItem as CommandDataFormat)!.Description!,
                (CommandsList.SelectedItem as CommandDataFormat)!.Phrase!,
                (CommandsList.SelectedItem as CommandDataFormat)!.Reference!,
                (CommandsList.SelectedItem as CommandDataFormat)!.ReplyMessage!);
        }

        private void importCommandsFromFile(object sender, RoutedEventArgs args)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".json",
                Filter = $"{Properties.Resources.commandsView_import_fileDialog_filter}|*.json"
            };

            var result = dialog.ShowDialog();

            if (result != true) return;
            File.Delete(".\\commands.json");
            Command.CommandsCollection = [];
            File.Copy(dialog.FileName, ".\\commands.json");
            CommandManager.deserialize();
            CommandsList.ItemsSource = Command.CommandsCollection;
        }
    }
}