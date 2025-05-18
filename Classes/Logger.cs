using System.IO;

namespace Leo.Classes
{
    public class Logger
    {
        private readonly string _path = @".\Logs\";
        private readonly DateTime _thisDay = DateTime.Now;

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
            await using var writer = new StreamWriter(_path, true);
            var str = $"[MESSAGE] {_thisDay:HH:mm:ss} - " + message;
            await writer.WriteLineAsync(str);
        }
        
        public async void warn(string message)
        {
            await using var writer = new StreamWriter(_path, true);
            var str = $"[WARNING] {_thisDay:HH:mm:ss} -  " + message;
            await writer.WriteLineAsync(str);
        }
        
        public async void error(string message)
        {
            await using var writer = new StreamWriter(_path, true);
            var str = $"[ERROR] {_thisDay:HH:mm:ss} -  " + message;
            await writer.WriteLineAsync(str);
        }
    }
}