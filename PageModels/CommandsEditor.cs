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
        private readonly MessageBox _messageBox = new();
        
        private static bool _editCommand;
        private static int _editingCommandId;
        
        public CommandsEditor()
        {
            InitializeComponent();
            _instance = this;
        }

        private void back(object sender, RoutedEventArgs e)
        {
            MainWindow.backPage();
        }

        public void setCommandData(int id, string name, string description, string Phrase, string Reference, string Reply)
        {
            _instance!.CommandNameTextBox.Text = name;
            _instance.CommandDescriptionTextBox.Text = description;
            _instance.CommandPhraseTextBox.Text = Phrase;
            _instance.CommandLinkTextBox.Text = Reference;
            _instance.CommandReplyTextBox.Text = Reply;
            
            _editCommand = true;
            _editingCommandId = id;
            Title.Content = name;
        }

        private async void writeNewCommand(object sender, RoutedEventArgs args)
        {
            if (_instance!.CommandNameTextBox.Text == string.Empty ||
                _instance.CommandDescriptionTextBox.Text == string.Empty ||
                _instance.CommandPhraseTextBox.Text == string.Empty ||
                _instance.CommandLinkTextBox.Text == string.Empty ||
                _instance.CommandReplyTextBox.Text == string.Empty)
            {
                _messageBox.showMessage(Properties.Resources.messageBox_messageSign, Properties.Resources.system_message2,
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
                        Type = "1",
                        Reference = _instance.CommandLinkTextBox.Text,
                        VoiceFile = ".\\Assets\\Voices\\open\\open1.wav",
                        ErrorNumber = "3",
                        ReplyMessage = _instance.CommandReplyTextBox.Text,
                    };
                    var json = JsonConvert.SerializeObject(commandDataFormat, Formatting.Indented);
                    Command.addCommand(_editingCommandId.ToString(),
                        _instance.CommandNameTextBox.Text, 
                        _instance.CommandDescriptionTextBox.Text,
                        "1", 
                        _instance.CommandLinkTextBox.Text, 
                        _instance.CommandPhraseTextBox.Text,
                        ".\\Assets\\Voices\\open\\open1.wav",
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
                        Type = "1",
                        Reference = _instance.CommandLinkTextBox.Text,
                        VoiceFile = ".\\Assets\\Voices\\open\\open1.wav",
                        ErrorNumber = "3",
                        ReplyMessage = _instance.CommandReplyTextBox.Text,
                    };
                    var json = JsonConvert.SerializeObject(commandDataFormat, Formatting.Indented);
                    Command.addCommand(Properties.Settings.Default.lastCommandId.ToString(),
                        _instance.CommandNameTextBox.Text, 
                        _instance.CommandDescriptionTextBox.Text,
                        "1", 
                        _instance.CommandLinkTextBox.Text, 
                        _instance.CommandPhraseTextBox.Text,
                        ".\\Assets\\Voices\\open\\open1.wav",
                        "3", 
                        _instance.CommandReplyTextBox.Text);
                    await writer.WriteLineAsync(json);
                    back(sender, args);
                }
            }
        }

        private void openFile(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".exe",
                Filter = $"{Properties.Resources.commandsEditor_fileDialog_filter}|*.exe"
            };

            var result = dialog.ShowDialog();
            
            if (result == true)
            {
                CommandLinkTextBox.Text = dialog.FileName;
            }
        }
    }
}