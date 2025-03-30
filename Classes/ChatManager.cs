using System.Collections.ObjectModel;
using System.IO;
using Leo.PageModels;
using Leo.Properties;
using Leo.WindowModels;
using Newtonsoft.Json;
using MessageBox = Leo.WindowModels.MessageBox;

namespace Leo.Classes
{
    public class MessagesJsonFormat
    {
        public string? Text { get; init; }
        public string? Time { get; init; }
        public string? Date { get; init; }
        public string? Alignment { get; init; }
        public bool IsDateVisible { get; init; }
        public string? Id { get; init; }
    }
    
    public class ChatManager
    {
        private readonly string _path = @".\data.json";
        private static readonly Logger Logger = new();
        private readonly MessageBox _messageBox = new();
        
        public ChatManager()
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
                    //File.SetAttributes(_path, FileAttributes.Hidden);
                    Properties.Settings.Default.messagesId = 0;
                    Properties.Settings.Default.nowDate = "01.01.01";
                    Properties.Settings.Default.Save();
                    Chat.ChatItems = new ObservableCollection<MessageData>();
                    Chat.NullMessages = true;
                }
            }
            else
            {
                Chat.ChatItems = new ObservableCollection<MessageData>();
                Chat.NullMessages = true;
            }
            
        }

        public string getFilePath()
        { return _path; }

        public async void serialize(string? text, string? alignment, string? time, string? date, bool isDateVisible, int id)
        {
            if (Properties.Settings.Default.notSaveMessages)
            {
                return;
            }
            
            await using StreamWriter writer = new StreamWriter(_path, true);
            MessagesJsonFormat messagesJsonFormat = new MessagesJsonFormat()
            {
                Text = text,
                Time = time,
                Date = date,
                Alignment = alignment,
                IsDateVisible = isDateVisible,
                Id = id.ToString()
            };
            string json = JsonConvert.SerializeObject(messagesJsonFormat, Formatting.Indented);
            
            await writer.WriteLineAsync(json);
        }

        public async void deserialize()
        {
            if (Properties.Settings.Default.notSaveMessages)
            { return; }
            
            try
            {
                using var reader = new StreamReader(_path);
                while (true)
                {
                    var line = "";
                    for (var i = 0; i <= 7; i++)
                    {
                        line += await reader.ReadLineAsync();
                    }
                    if (string.IsNullOrEmpty(line))
                    {
                        break;
                    }
                    MessagesJsonFormat? md = JsonConvert.DeserializeObject<MessagesJsonFormat>(line);
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
                                    Chat.clearChat();
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
                            Chat.addMessage(md?.Text, md?.Alignment, md?.Time, md?.Date, md!.IsDateVisible);
                        }
                        else
                        {
                            _messageBox.Results = 0;
                            break;
                        } 
                    }
                    else
                    {
                        Chat.addMessage(md?.Text, md?.Alignment, md?.Time, md?.Date, md!.IsDateVisible);
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