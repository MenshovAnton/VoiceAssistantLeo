using System.Collections.ObjectModel;
using System.IO;
using Leo.PageModels;
using Leo.Properties;
using Leo.WindowModels;
using Newtonsoft.Json;

namespace Leo.Classes
{
    public abstract class CommandDataManager
    {
        private const string Path = @".\commands.json";
        private static readonly Logger Logger = new();
        private static readonly MessageBox MessageBox = new();

        protected CommandDataManager()
        {
            if (Properties.Settings.Default.notSaveMessages == false)
            {
                try
                {
                    var file = File.Open(Path, FileMode.Open);
                    file.Close();
                }
                catch
                {
                    var file = File.Create(Path);
                    file.Close();
                }
            }
            else
            {
                Command.CommandsCollection = [];
                Chat.NullMessages = true;
            }
        }

        public static void saveCommands(Collection<CommandDataFormat> commands)
        {
            File.Delete(Path);
            var file = File.Open(Path, FileMode.Create);
            file.Close();
            foreach (var command in commands)
            {
                serialize(command.Id, command.Name, command.Description, command.Phrase!, command.Type, command.Reference, 
                    command.VoiceFile, command.ErrorNumber, command.ReplyMessage);
            }
        }

        private static async void serialize(string? id, string? name, string? description, string phrase, string? type, 
            string? reference, string? voiceFile, string? errorNumber, string? replyMessage)
        {
            try
            {
                await using var writer = new StreamWriter(Path, true);
                var commandDataFormat = new CommandDataFormat()
                {
                    Id = id,
                    Name = name,
                    Description = description,
                    Phrase = phrase,
                    Type = type,
                    Reference = reference,
                    VoiceFile = voiceFile,
                    ErrorNumber = errorNumber,
                    ReplyMessage = replyMessage
                };
                var json = JsonConvert.SerializeObject(commandDataFormat, Formatting.Indented);
            
                await writer.WriteLineAsync(json);
                writer.Close();
            }
            catch (Exception ex)
            {
                Logger.error("Async error in serialize commands\n" + ex);
                MessageBox.showMessage(Resources.messageBox_errorSign, Resources.system_message5,
                    MessageBox.MessageBoxType.Error, MessageBox.MessageBoxButtons.Ok);
            }
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
                    var md = JsonConvert.DeserializeObject<CommandDataFormat>(line);
                    Command.addCommand(md?.Id!, md?.Name, md?.Description, md?.Type!, md?.Reference!, 
                        md?.Phrase, md?.VoiceFile!, md?.ErrorNumber!, md?.ReplyMessage!);
                }
            }
            catch (Exception ex)
            {
                Logger.error("Failed to load commands\n" + ex);
                MessageBox.showMessage(Resources.messageBox_errorSign, Resources.system_error7,
                    MessageBox.MessageBoxType.Error, MessageBox.MessageBoxButtons.Ok);
            }
        }
    }
}