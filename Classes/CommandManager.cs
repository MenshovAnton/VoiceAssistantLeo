using System.Collections.ObjectModel;
using System.IO;
using Leo.PageModels;
using Leo.WindowModels;

namespace Leo.Classes
{
    public class CommandData
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? MethodRef { get; set; }
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
                    //File.SetAttributes(_path, FileAttributes.Hidden);
                    Properties.Settings.Default.messagesId = 0;
                    Properties.Settings.Default.nowDate = "01.01.01";
                    Properties.Settings.Default.Save();
                    //MainWindow.ChatCollection = new ObservableCollection<Chat.Messages>();
                    Chat.NullMessages = true;
                }
            }
            else
            {
                //MainWindow.ChatCollection = new ObservableCollection<Chat.Messages>();
                Chat.NullMessages = true;
            }
        }
    }
}