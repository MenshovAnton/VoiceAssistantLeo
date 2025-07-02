using System.IO;
using Leo.PageModels;
using Leo.Properties;
using Newtonsoft.Json;
using MessageBox = Leo.WindowModels.MessageBox;

namespace Leo.Classes
{
    public class ChatDataManager
    {
        private const string Path = @".\data.json";
        private static readonly Logger Logger = new();
        private static readonly MessageBox MessageBox = new();
        
        public ChatDataManager()
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
                }
            }
            else
            {
                Message.MessagesCollection = [];
            }
        }

        public static string getFilePath()
        {
            return Path;
        }

        public static async void serialize(string? text, string? alignment, string? time, string? date, bool isDateVisible, int id)
        {
            try
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
            catch (Exception ex)
            {
                Logger.error("Async error in serialize messages\n" + ex);
                
                MessageBox.showMessage(Resources.messageBox_errorSign, Resources.system_message5,
                    MessageBox.MessageBoxType.Error, MessageBox.MessageBoxButtons.Ok);
            }
        }

        public static async void deserialize()
        {
            try
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
                            
                            MessageBox.showMessage(Resources.messageBox_messageSign, Resources.system_message1,
                                MessageBox.MessageBoxType.Info, MessageBox.MessageBoxButtons.OkCancel);
                            
                            await Task.Run(() =>
                            {
                                while (MessageBox.IsOpened)
                                {
                                    if (MessageBox.Results == 1)
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
                            
                            if (MessageBox.Results == 0)
                            {
                                Chat.addMessageItem(md?.MessageText, md?.Alignment, md?.Time, md?.Date, md!.IsDateVisible);
                            }
                            else
                            {
                                MessageBox.Results = 0;
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
                    Logger.error("Failed to load recent messages\n" + ex);
                    
                    MessageBox.showMessage(Resources.messageBox_errorSign, Resources.system_error4,
                        MessageBox.MessageBoxType.Error, MessageBox.MessageBoxButtons.Ok);
                }
            }
            catch (Exception ex)
            {
                Logger.error("Async error in deserialize messages\n" + ex);
                
                MessageBox.showMessage(Resources.messageBox_errorSign, Resources.system_message5,
                    MessageBox.MessageBoxType.Error, MessageBox.MessageBoxButtons.Ok);
            }
        }
    }
}