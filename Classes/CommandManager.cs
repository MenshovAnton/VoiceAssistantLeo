using System.Collections.ObjectModel;
using System.IO;
using Leo.PageModels;
using Leo.Properties;
using Leo.WindowModels;
using Newtonsoft.Json;

namespace Leo.Classes
{
    public class CommandJsonFormat
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Phrase { get; set; }
        public string? Type { get; set; }
        public string? Reference { get; set; }
        public string? VoiceFile { get; set; }
        public string? ErrorNumber { get; set; }
        public string? ReplyMessage { get; set; }
    }

    public class CommandManager
    {
        private const string Path = @".\commands.json";
        private static readonly Logger Logger = new();
        private static readonly MessageBox _messageBox = new();

        public CommandManager()
        {
            if (Properties.Settings.Default.notSaveMessages == false)
            {
                try
                {
                    FileStream file = File.Open(Path, FileMode.Open);
                    file.Close();
                }
                catch
                {
                    FileStream file = File.Create(Path);
                    file.Close();
                }
            }
            else
            {
                Vosk.Commands = new ObservableCollection<CommandData>();
                Chat.NullMessages = true;
            }
        }
        
        public static async void serialize(int id, string name, string? description, string? type, string? reference, string? voiceFile, string? errorNumber, string? replyMessage)
        {
            if (Properties.Settings.Default.notSaveMessages)
            {
                return;
            }
            
            await using StreamWriter writer = new StreamWriter(Path, true);
            CommandJsonFormat commandJson = new CommandJsonFormat()
            {
                Id = id.ToString(),
                Name = name,
                Description = description,
                Type = type,
                Reference = reference,
                VoiceFile = voiceFile,
                ErrorNumber = errorNumber,
                ReplyMessage = replyMessage
            };
            string json = JsonConvert.SerializeObject(commandJson, Formatting.Indented);
            
            await writer.WriteLineAsync(json);
        }

        public static async void deserialize()
        {
            try
            {
                using var reader = new StreamReader(Path);
                while (true)
                {
                    var line = "";
                    for (var i = 0; i <= 10; i++)
                    {
                        line += await reader.ReadLineAsync();
                    }
                    if (string.IsNullOrEmpty(line))
                    {
                        break;
                    }
                    CommandJsonFormat? md = JsonConvert.DeserializeObject<CommandJsonFormat>(line);
                    Vosk.addCommand(md?.Name, md?.Description, md?.Type!, md?.Reference!, md?.Phrase, md?.VoiceFile!, md?.ErrorNumber!, md?.ReplyMessage!);
                }
            }
            catch (Exception ex)
            {
                Logger.error("Leo failed to load commands " + ex);
                _messageBox.showMessage(Resources.messageBox_errorSign, Resources.system_error7,
                    MessageBox.MessageBoxType.Error, MessageBox.MessageBoxButtons.Ok);
            }
        }
    }
}