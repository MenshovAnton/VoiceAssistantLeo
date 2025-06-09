using System.Collections.ObjectModel;
using System.IO;
using Leo.PageModels;

namespace Leo.Classes;

public class MessageDataFormat
{
    public string? Id { get; init; }
    public string? MessageText { get; init; }
    public string? Time { get; init; }
    public string? Date { get; init; }
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public int Length { get; set; }
    public string? Alignment { get; init; }
    public bool IsDateVisible { get; init; }
}

public abstract class Message
{
    public static ObservableCollection<MessageDataFormat>? MessagesCollection { get; set; }

    public static void addMessage(string text, int length, string alignment, bool isDateVisible)
    {
        
        
        MessagesCollection!.Add(new MessageDataFormat
        {
            MessageText = text,
            Time = DateTime.Now.ToShortTimeString(),
            Length = length,
            Alignment = alignment,
            Date = DateTime.Now.ToShortDateString(),
            IsDateVisible = isDateVisible
        });

        Properties.Settings.Default.messagesId += 1;
        Properties.Settings.Default.Save();
            
        ChatDataManager.serialize(text, alignment, DateTime.Now.ToShortTimeString(), 
            DateTime.Now.ToShortDateString(), isDateVisible, Properties.Settings.Default.messagesId);
    }

    public static void addMessage(string text, string time, int length, string alignment, string date, bool isDateVisible)
    {
        MessagesCollection!.Add(new MessageDataFormat
        {
            MessageText = text,
            Time = time,
            Length = length,
            Alignment = alignment,
            Date = date,
            IsDateVisible = isDateVisible
        });
    }

    public static void clearMessages()
    {
        Properties.Settings.Default.messagesId = 0;
        Properties.Settings.Default.nowDate = "01.01.2001";
        Properties.Settings.Default.Save();
        File.Delete(ChatDataManager.getFilePath());
        var file = File.Create(ChatDataManager.getFilePath());
        file.Close();
        MessagesCollection = [];
        Chat.NullMessages = true;
    }
}