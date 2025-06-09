using System.Diagnostics;
using System.IO;
using System.Windows;
using Leo.Classes;
using Leo.WindowModels;
using Newtonsoft.Json;
using MessageBox = Leo.WindowModels.MessageBox;

namespace Leo.PageModels
{
    public partial class CommandsEditor
    {
        private const string Path = @".\commands.json";
        private static CommandsEditor? _instance;
        private readonly Logger _logger = new();
        private readonly MessageBox _messageBox = new();
        private static readonly VoicesManager VoicesManager = new();
        
        private static bool _editCommand;
        private static int _editingCommandId;
        
        
        public CommandsEditor()
        {
            InitializeComponent();
            _instance = this;
            CommandVoiceComboBox.ItemsSource = VoicesManager.getVoiceFilesList();
        }

        private void back(object sender, RoutedEventArgs e)
        {
            MainWindow.backPage();
        }

        public void setCommandData(int id, string name, string description, string phrase, string reference, string reply, int type, string? voiceFile)
        {
            _instance!.CommandNameTextBox.Text = name;
            _instance.CommandDescriptionTextBox.Text = description;
            _instance.CommandPhraseTextBox.Text = phrase;
            _instance.CommandLinkTextBox.Text = reference;
            _instance.CommandReplyTextBox.Text = reply;
            _instance.CommandTypeComboBox.SelectedIndex = type - 1;
            _instance.CommandVoiceComboBox.SelectedIndex = VoicesManager.getIndexOfVoiceFile(voiceFile!);
            
            _editCommand = true;
            _editingCommandId = id;
            Title.Content = name;
        }

        private async void writeNewCommand(object sender, RoutedEventArgs args)
        {
            try
            {
                if (_instance!.CommandNameTextBox.Text == string.Empty ||
                    _instance.CommandDescriptionTextBox.Text == string.Empty ||
                    _instance.CommandPhraseTextBox.Text == string.Empty ||
                    _instance.CommandLinkTextBox.Text == string.Empty ||
                    _instance.CommandReplyTextBox.Text == string.Empty ||
                    _instance.CommandTypeComboBox.SelectedIndex == -1 ||
                    _instance.CommandVoiceComboBox.SelectedIndex == -1)
                {
                    _messageBox.showMessage(Properties.Resources.messageBox_messageSign, Properties.Resources.system_message2,
                        MessageBox.MessageBoxType.Info, MessageBox.MessageBoxButtons.Ok);
                }

                if (!Scripts.isValidPathOrUrl(_instance.CommandLinkTextBox.Text) && _instance.CommandLinkTextBox.Text != string.Empty)
                {
                    _messageBox.showMessage(Properties.Resources.messageBox_messageSign, Properties.Resources.system_message4,
                        MessageBox.MessageBoxType.Info, MessageBox.MessageBoxButtons.Ok);
                }
                else
                {
                    if (_editCommand)
                    {
                        Command.deleteCommand(_editingCommandId);
                
                        await using var writer = new StreamWriter(Path, true);
                        var commandDataFormat = new CommandDataFormat()
                        {
                            Id = _editingCommandId.ToString(),
                            Name = _instance.CommandNameTextBox.Text,
                            Description = _instance.CommandDescriptionTextBox.Text,
                            Phrase = _instance.CommandPhraseTextBox.Text,
                            Type = (_instance.CommandTypeComboBox.SelectedIndex + 1).ToString(),
                            Reference = _instance.CommandLinkTextBox.Text,
                            VoiceFile = _instance.CommandVoiceComboBox.SelectedItem.ToString(),
                            ErrorNumber = "3",
                            ReplyMessage = _instance.CommandReplyTextBox.Text,
                        };
                        var json = JsonConvert.SerializeObject(commandDataFormat, Formatting.Indented);
                        Command.addCommand(_editingCommandId.ToString(),
                            _instance.CommandNameTextBox.Text, 
                            _instance.CommandDescriptionTextBox.Text,
                            (_instance.CommandTypeComboBox.SelectedIndex + 1).ToString(), 
                            _instance.CommandLinkTextBox.Text, 
                            _instance.CommandPhraseTextBox.Text,
                            _instance.CommandVoiceComboBox.SelectedItem.ToString(),
                            "3", 
                            _instance.CommandReplyTextBox.Text);
                        await writer.WriteLineAsync(json);
                        back(sender, args);
                    }
                    else
                    {
                        Properties.Settings.Default.lastCommandId++;
                        Properties.Settings.Default.Save();
                
                        await using var writer = new StreamWriter(Path, true);
                        var commandDataFormat = new CommandDataFormat()
                        {
                            Id = Properties.Settings.Default.lastCommandId.ToString(),
                            Name = _instance.CommandNameTextBox.Text,
                            Description = _instance.CommandDescriptionTextBox.Text,
                            Phrase = _instance.CommandPhraseTextBox.Text,
                            Type = (_instance.CommandTypeComboBox.SelectedIndex + 1).ToString(),
                            Reference = _instance.CommandLinkTextBox.Text,
                            VoiceFile = _instance.CommandVoiceComboBox.SelectedItem.ToString(),
                            ErrorNumber = "3",
                            ReplyMessage = _instance.CommandReplyTextBox.Text,
                        };
                        var json = JsonConvert.SerializeObject(commandDataFormat, Formatting.Indented);
                        Command.addCommand(Properties.Settings.Default.lastCommandId.ToString(),
                            _instance.CommandNameTextBox.Text, 
                            _instance.CommandDescriptionTextBox.Text,
                            (_instance.CommandTypeComboBox.SelectedIndex + 1).ToString(), 
                            _instance.CommandLinkTextBox.Text, 
                            _instance.CommandPhraseTextBox.Text,
                            _instance.CommandVoiceComboBox.SelectedItem.ToString(),
                            "3", 
                            _instance.CommandReplyTextBox.Text);
                        await writer.WriteLineAsync(json);
                        back(sender, args);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.error("Async error in write command\n" + ex);
                _messageBox.showMessage( Properties.Resources.messageBox_errorSign, Properties.Resources.system_message5,
                    MessageBox.MessageBoxType.Error, MessageBox.MessageBoxButtons.Ok);
            }
        }

        private void openFile(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".exe",
                Filter = $"{Properties.Resources.commandsEditorPage_fileDialog_filter}|*.exe"
            };

            var result = dialog.ShowDialog();
            
            if (result == true)
            {
                CommandLinkTextBox.Text = dialog.FileName;
            }
        }
        
        private void addVoiceFile(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".wav",
                Filter = $"{Properties.Resources.commandsEditorPage_addVoice_fileDialog_filter}|*.wav"
            };

            var result = dialog.ShowDialog();
        }
        
        private void openVoices(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", @".\Assets\Voices");
        }
    }
}