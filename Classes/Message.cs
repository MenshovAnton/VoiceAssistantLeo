using System.Collections.ObjectModel;
using System.IO;
using Leo.PageModels;

namespace Leo.Classes;

public class MessageDataFormat
{
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    public string? Id { get; init; }
    public string? MessageText { get; init; }
    public string? Time { get; init; }
    public string? Date { get; init; }
    public int Length { get; set; }
    public string? Alignment { get; init; }
    public bool IsDateVisible { get; init; }
    // ReSharper restore UnusedAutoPropertyAccessor.Global
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
            
        ChatManager.serialize(text, alignment, DateTime.Now.ToShortTimeString(), 
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
        Properties.Settings.Default.nowDate = "01.01.01";
        Properties.Settings.Default.Save();
        File.Delete(ChatManager.getFilePath());
        var file = File.Create(ChatManager.getFilePath());
        file.Close();
        MessagesCollection = [];
        Chat.NullMessages = true;
    }
}