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
        private readonly string _path = @".\commands.json";
        private static readonly Logger Logger = new();
        private readonly MessageBox _messageBox = new();

        public CommandManager()
        {
            if (Properties.Settings.Default.notSaveMessages == false)
            {
                try
                {
                    FileStream file = File.Open(_path, FileMode.Open);
                    file.Close();
                }
                catch
                {
                    FileStream file = File.Create(_path);
                    file.Close();
                }
            }
            else
            {
                Vosk.Commands = new ObservableCollection<CommandData>();
                Chat.NullMessages = true;
            }
        }
        
        public async void serialize(int id, string name, string? methodRef)
        {
            if (Properties.Settings.Default.notSaveMessages)
            {
                return;
            }
            
            await using StreamWriter writer = new StreamWriter(_path, true);
            CommandJsonFormat commandJson = new CommandJsonFormat()
            {
                Id = id.ToString(),
                Name = name,
                Reference = methodRef
            };
            string json = JsonConvert.SerializeObject(commandJson, Formatting.Indented);
            
            await writer.WriteLineAsync(json);
        }

        public async void deserialize()
        {
            try
            {
                using var reader = new StreamReader(_path);
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
                    Vosk.addCommand(md?.Type!, md?.Reference!, md?.Phrase, md?.VoiceFile!, md?.ErrorNumber!, md?.ReplyMessage!);
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