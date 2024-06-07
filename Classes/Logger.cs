﻿using System.IO;

namespace VA_Leo.Classes
{
    public class Logger
    {
        private readonly string _path = ".\\logs\\";
        private readonly DateTime _thisDay = DateTime.Now;

        public Logger()
        {
            var date = _thisDay.ToString("MM/dd/yyyy").Replace('.', '-').Replace('/', '-');
            
            _path += date + ".log";

            try
            {
                FileStream file = File.Open(_path, FileMode.Open);
                file.Close();
            }
            catch
            {
                FileStream file = File.Create(_path);
                file.Close();
            };
        }
        
        public async void message(string message)
        {
            await using StreamWriter writer = new StreamWriter(_path, true);
            string str = $"[MESSAGE] {_thisDay:HH:mm:ss} - " + message;
            await writer.WriteLineAsync(str);
        }
        
        public async void warn(string message)
        {
            await using StreamWriter writer = new StreamWriter(_path, true);
            string str = $"[WARNING] {_thisDay:HH:mm:ss} -  " + message;
            await writer.WriteLineAsync(str);
        }
        
        public async void error(string message)
        {
            await using StreamWriter writer = new StreamWriter(_path, true);
            string str = $"[ERROR] {_thisDay:HH:mm:ss} -  " + message;
            await writer.WriteLineAsync(str);
        }
        
        public async void critical(string message)
        {
            await using StreamWriter writer = new StreamWriter(_path, true);
            string str = $"[CRITICAL ERROR] {_thisDay:HH:mm:ss} - " + message;
            await writer.WriteLineAsync(str);
        }
    }
}