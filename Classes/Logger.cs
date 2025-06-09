using System.IO;
using Leo.Properties;
using Leo.WindowModels;

namespace Leo.Classes
{
    public class Logger
    {
        private readonly string _path = @".\Logs\";
        private readonly DateTime _thisDay = DateTime.Now;
        private readonly MessageBox _messageBox = new();

        public Logger()
        {
            var date = _thisDay.ToString("MM/dd/yyyy").Replace('.', '-').Replace('/', '-');
            _path += date + ".log";

            try
            {
                var file = File.Open(_path, FileMode.Open);
                file.Close();
            }
            catch
            {
                var file = File.Create(_path);
                file.Close();
            }
        }
        
        public async void message(string message)
        {
            try
            {
                await using var writer = new StreamWriter(_path, true);
                var str = $"[MESSAGE] {_thisDay:HH:mm:ss} - " + message;
                await writer.WriteLineAsync(str);
            }
            catch
            {
                _messageBox.showMessage(Resources.messageBox_errorSign, Resources.system_message5,
                    MessageBox.MessageBoxType.Error, MessageBox.MessageBoxButtons.Ok);
            }
        }
        
        public async void warn(string message)
        {
            try
            {
                await using var writer = new StreamWriter(_path, true);
                var str = $"[WARNING] {_thisDay:HH:mm:ss} -  " + message;
                await writer.WriteLineAsync(str);
            }
            catch
            {
                _messageBox.showMessage(Resources.messageBox_errorSign, Resources.system_message5,
                    MessageBox.MessageBoxType.Error, MessageBox.MessageBoxButtons.Ok);
            }
        }
        
        public async void error(string message)
        {
            try
            {
                await using var writer = new StreamWriter(_path, true);
                var str = $"[ERROR] {_thisDay:HH:mm:ss} -  " + message;
                await writer.WriteLineAsync(str);
            }
            catch
            {
                _messageBox.showMessage(Resources.messageBox_errorSign, Resources.system_message5,
                    MessageBox.MessageBoxType.Error, MessageBox.MessageBoxButtons.Ok);
            }
        }
    }
}