using System.Collections.ObjectModel;

namespace Leo.Classes;

public class CommandDataFormat
{
    public string? Id { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? Phrase { get; init; }
    public string? Reference { get; init; }
    public string? Type { get; init; }
    public string? VoiceFile { get; init; }
    public string? ErrorNumber { get; init; }
    public string? ReplyMessage { get; init; }
}

public abstract class Command
{
    public static ObservableCollection<CommandDataFormat>? CommandsCollection { get; set; }
    
    public static void addCommand(string id ,string? name, string? description,string type, string methodRef, 
        string? phrase, string? voiceFile, string? errorNumber, string? replyMessage)
    {
        CommandsCollection!.Add(new CommandDataFormat()
        {
            Id = id,
            Name = name,
            Description = description,
            Phrase = phrase,
            Reference = methodRef,
            Type = type,
            VoiceFile = voiceFile,
            ErrorNumber = errorNumber,
            ReplyMessage = replyMessage
        });
    }
    
    public static void deleteCommand(int id)
    {
        CommandsCollection!.RemoveAt(id);
        CommandDataManager.saveCommands(CommandsCollection);
    }
}