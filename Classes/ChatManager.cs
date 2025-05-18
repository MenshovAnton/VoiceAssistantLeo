using System.IO;
using Leo.PageModels;
using Leo.Properties;
using Newtonsoft.Json;
using MessageBox = Leo.WindowModels.MessageBox;

namespace Leo.Classes
{
    public class ChatManager
    {
        private const string Path = @".\data.json";
        private static readonly Logger Logger = new();
        private readonly MessageBox _messageBox = new();
        
        public ChatManager()
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
                    Properties.Settings.Default.messagesId = 0;
                    Properties.Settings.Default.nowDate = "01.01.01";
                    Properties.Settings.Default.Save();
                    Message.MessagesCollection = [];
                    Chat.NullMessages = true;
                }
            }
            else
            {
                Message.MessagesCollection = [];
                Chat.NullMessages = true;
            }
        }

        public static string getFilePath()
        {
            return Path;
        }

        public static async void serialize(string? text, string? alignment, string? time, string? date, bool isDateVisible, int id)
        {
            if (Properties.Settings.Default.notSaveMessages)
            {
                return;
            }
            
            await using var writer = new StreamWriter(Path, true);
            var messageData = new MessageDataFormat()
            {
                MessageText = text,
                Time = time,
                Date = date,
                Alignment = alignment,
                IsDateVisible = isDateVisible,
                Id = id.ToString()
            };
            var json = JsonConvert.SerializeObject(messageData, Formatting.Indented);
            
            await writer.WriteLineAsync(json);
        }

        public async void deserialize()
        {
            if (Properties.Settings.Default.notSaveMessages)
            {
                return;
            }
            
            try
            {
                using var reader = new StreamReader(Path);
                while (true)
                {
                    var line = "";
                    for (var i = 0; i <= 8; i++)
                    {
                        line += await reader.ReadLineAsync();
                    }
                    if (string.IsNullOrEmpty(line))
                    {
                        break;
                    }
                    var md = JsonConvert.DeserializeObject<MessageDataFormat>(line);
                    if (int.Parse(md?.Id!) >= 10000 && Properties.Settings.Default.offLotMessageWarn == false)
                    {
                        Logger.message("Chat messages have reached 10,000 and need clearing");
                        _messageBox.showMessage(Resources.messageBox_messageSign, Resources.system_message1,
                            MessageBox.MessageBoxType.Info, MessageBox.MessageBoxButtons.OkCancel);
                        await Task.Run(() =>
                        {
                            while (_messageBox.IsOpened)
                            {
                                if (_messageBox.Results == 1)
                                {
                                    reader.Close();
                                    Message.clearMessages();
                                }
                                else
                                {
                                    continue;
                                }
                                break;
                            }
                        });
                        if (_messageBox.Results == 0)
                        {
                            Chat.addMessageItem(md?.MessageText, md?.Alignment, md?.Time, md?.Date, md!.IsDateVisible);
                        }
                        else
                        {
                            _messageBox.Results = 0;
                            break;
                        } 
                    }
                    else
                    {
                        Chat.addMessageItem(md?.MessageText, md?.Alignment, md?.Time, md?.Date, md!.IsDateVisible);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.error("Leo failed to load recent messages " + ex);
                _messageBox.showMessage(Resources.messageBox_errorSign, Resources.system_error4,
                    MessageBox.MessageBoxType.Error, MessageBox.MessageBoxButtons.Ok);
            }
        }
    }
}